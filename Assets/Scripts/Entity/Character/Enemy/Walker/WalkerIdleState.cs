using System.Collections;
using UnityEngine;

namespace MyMetroidVania.Entity.Character.Enemy.Walker
{
    /// <summary>
    /// ウォーカーの待機ステートを管理
    /// </summary>
    public class WalkerIdleState : EnemyState<EnemyWalker>
    {
        public WalkerIdleState(EnemyWalker enemy) : base(enemy) { }

        private WalkerChaseState _chaseState = null;
        private WalkerChaseState ChaseState
        {
            get
            {
                if (_chaseState == null)
                {
                    // 存在しなかった場合は生成して返す
                    return _chaseState = new WalkerChaseState(_owner);
                }
                else
                {
                    return _chaseState;
                }
            }
        }
        private SleepState _sleepState = null;
        private SleepState SleepState
        {
            get
            {
                if (_sleepState == null)
                {
                    // 存在しなかった場合は生成して返す
                    return _sleepState = new SleepState(_owner, this);
                }
                else
                {
                    return _sleepState;
                }
            }
        }

        public override void Enter()
        {
            _owner.StartCoroutine(IdleRoutine());
        }

        /// <summary>
        /// ステートの状態遷移を監視
        /// </summary>
        protected override void OnTick()
        {
            Debug.Log("ウォーカーの待機ステート行動中");
            if (_owner.IsStun)
            {
                // ダメージを受けてスタンした
                _owner.ChangeState(StunState);
                return;
            }

            if (_owner.IsPlayerDetected())
            {
                // プレイヤーを検知した
                _owner.ChangeState(ChaseState);
                return;
            }

            if (!_owner.IsVisible())
            {
                // 画面外に出た
                _owner.ChangeState(SleepState);
                return;
            }
        }

        public override void Exit()
        {
            // コルーチンは全てStateクラスから発火される想定なので、全てのコルーチンを止めてよい。
            _owner.StopAllCoroutines();
            _owner.StopMove();
        }


        private IEnumerator IdleRoutine()
        {
            while (true)
            {
                yield return _owner.OnMove();
            }
        }
    }
}