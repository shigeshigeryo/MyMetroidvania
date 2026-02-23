using MyMetroidVania.Entity.Character.Enemy;
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
        [SerializeField] private List<EnemyBase> _bossList = new List<EnemyBase>();

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
            Unlock();

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
    }
}