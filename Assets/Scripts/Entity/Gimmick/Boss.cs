using MyMetroidVania.Data.ScriptableObjects;
using MyMetroidVania.Entity.Character.Enemy;
using MyMetroidVania.System;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyMetroidVania.Entity.Gimmick
{
    public class Boss : GimmickBase, IUnlockKey
    {
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
        [SerializeField] private LayerMask _playerLayer;

        public event Action OnUnlocked;

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
        /// ボスBGMを流す
        /// </summary>
        private void ReturnBackupSound()
        {
            if (!_isPlayingBossSound) return;

            AudioManager.Instance.ReturnBackupBgm();
            _isPlayingBossSound = false;
        }

        /// <summary>
        /// ボス死亡時に呼ばれる
        /// </summary>
        private void CheckConditions()
        {
            for (int i = 0; i < _bossList.Count; i++)
            {
                // まだ生存しているボスがいる
                if (!_bossList[i].IsDead) return;
            }

            Debug.Log("ボス討伐確認");
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
            Debug.Log("アンロック処理");
            OnUnlocked?.Invoke();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_currentState == State.Beaten) return;

            if (1 << collision.gameObject.layer == _playerLayer)
            {
                PlayBossSound();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (_currentState == State.Beaten) return;

            if (1 << collision.gameObject.layer == _playerLayer)
            {
                ReturnBackupSound();
            }
        }
    }
}