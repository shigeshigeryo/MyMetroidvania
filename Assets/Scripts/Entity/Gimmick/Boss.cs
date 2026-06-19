using MyMetroidVania.Entity.Character.Enemy;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyMetroidVania.Entity.Gimmick
{
    /// <summary>
    /// ボスギミックを管理
    /// </summary>
    public class Boss : GimmickBase, IUnlockKey
    {
        /// <summary>
        /// ボスの状態
        /// </summary>
        [Serializable]
        public enum State
        {
            None,
            Beaten // 勝利済み
        }
        private State _currentState = State.None;
        [SerializeField] private string _bossSoundName = "BGM_BossBattle";
        private SoundData _bossSound = null;
        [SerializeField] private string _winSoundName = "Jingle_Win";
        private SoundData _winSound = null;
        private bool _isPlayingBossSound = false;
        [SerializeField] private List<EnemyBase> _bossList = new List<EnemyBase>();
        private static int _playerLayer = -1;
        private static int PlayerLayer
        {
            get
            {
                if (_playerLayer == -1)
                {
                    _playerLayer = LayerMask.NameToLayer("Player");
                }
                return _playerLayer;
            }
        }

        /// <summary>
        /// ボスが倒されアンロックするイベント
        /// </summary>
        public event Action OnUnlocked;

        /// <summary>
        /// 初期化処理
        /// </summary>
        public override void InitializeState()
        {
            if (_stateData.State == (int)State.Beaten)
            {
                _currentState = State.Beaten;
                DestroyBosses();
                return;
            }

            for (int i = 0; i < _bossList.Count; i++)
            {
                _bossList[i].OnCompletedDeadAnimationEvent += CheckConditions;
            }

            _bossSound = AudioManager.Instance.GetBgm(_bossSoundName);
            _winSound = AudioManager.Instance.GetBgm(_winSoundName);
        }

        /// <summary>
        /// ボスBGMを流す
        /// </summary>
        private void PlayBossSound()
        {
            if (_isPlayingBossSound) return;

            AudioManager.Instance.InterruptPlayBgm(_bossSound);
            _isPlayingBossSound = true;
        }
        /// <summary>
        /// バックアップしていたBGMを流す
        /// </summary>
        private void ReturnBackupSound()
        {
            if (!_isPlayingBossSound) return;

            AudioManager.Instance.ReturnBackupBgm();
            _isPlayingBossSound = false;
        }

        /// <summary>
        /// ボス死亡時に発火する処理
        /// </summary>
        private void CheckConditions()
        {
            for (int i = 0; i < _bossList.Count; i++)
            {
                // まだ生存しているボスがいる
                if (!_bossList[i].IsDead) return;
            }

            // ボスをすべて討伐した場合
            DestroyBosses();

            // ジングルを鳴らした後にボス前のBGMに戻す
            AudioManager.Instance.PlayJingle(_winSound, () =>
            {
                ReturnBackupSound();
                Unlock();
            });

            _currentState = State.Beaten;
            _stateData.SetState((int)State.Beaten);
        }

        /// <summary>
        /// ボスが撃破を削除する処理
        /// </summary>
        private void DestroyBosses()
        {
            for (int i = 0; i < _bossList.Count; i++)
            {
                _bossList[i].OnCompletedDeadAnimationEvent -= CheckConditions;
                Destroy(_bossList[i].gameObject);
            }

            _bossList.Clear();
        }

        /// <summary>
        /// 何かのロックを解除する処理
        /// 初期化時は呼ばない（ギミックの情報は既にセーブされているため）
        /// </summary>
        public void Unlock()
        {
            OnUnlocked?.Invoke();
        }

        /// <summary>
        /// トリガー処理
        /// ボス悲劇派の場合に戦闘BGMを流す
        /// </summary>
        /// <param name="collision">トリガー対象</param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_currentState == State.Beaten) return;

            if (collision.gameObject.layer == PlayerLayer)
            {
                PlayBossSound();
            }
        }
        
        /// <summary>
        /// トリガーexit処理
        /// 戦闘BGMから元のBGMに戻す
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (_currentState == State.Beaten) return;

            if (collision.gameObject.layer == PlayerLayer)
            {
                ReturnBackupSound();
            }
        }
    }
}
