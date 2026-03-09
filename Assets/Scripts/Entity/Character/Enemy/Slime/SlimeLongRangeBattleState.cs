using System.Collections;
using UnityEngine;

namespace MyMetroidVania.Entity.Character.Enemy.Slime
{
    public class SlimeLongRangeBattleState : EnemyState<EnemySlime>
    {
        private bool _isAttacking = false;
        public SlimeLongRangeBattleState(EnemySlime enemy) : base(enemy) { }

        private SlimeIdleState _idleState = null;
        private SlimeIdleState IdleState
        {
            get
            {
                if (_idleState == null)
                {
                    // 存在しなかった場合は生成して返す
                    return _idleState = new SlimeIdleState(_owner);
                }
                else
                {
                    return _idleState;
                }
            }
        }

        private SlimeCloseRangeBattleState _closeRangeBattleState = null;
        private SlimeCloseRangeBattleState CloseRangeBattleState
        {
            get
            {
                if (_closeRangeBattleState == null)
                {
                    // 存在しなかった場合は生成して返す
                    return _closeRangeBattleState = new SlimeCloseRangeBattleState(_owner);
                }
                else
                {
                    return _closeRangeBattleState;
                }
            }
        }

        /// <summary>
        /// ステートに遷移時に発火
        /// </summary>
        public override void Enter()
        {
            _isAttacking = false;
            _owner.StartCoroutine(LongRangeBattleRoutine());
        }

        /// <summary>
        /// ステートの状態遷移を監視
        /// </summary>
        protected override void OnTick()
        {
            if (_owner.IsStun)
            {
                // スタンしないためスタン状態を回復する
                _owner.RecoverStun();
            }

            if (!_owner.IsPlayerDetected())
            {
                // プレイヤーが検知外に出た
                _owner.ChangeState(IdleState);
                return;
            }

            if (!_isAttacking && _owner.IsPlayerInRange())
            {
                // 攻撃中にステートの変更を行わない
                // プレイヤーが近距離攻撃射程内にいる
                _owner.ChangeState(CloseRangeBattleState);
                return;
            }
        }

        /// <summary>
        /// 次のステートに遷移する前に発火
        /// </summary>
        public override void Exit()
        {
            // コルーチンは全てStateクラスから発火される想定なので、全てのコルーチンを止めてよい。
            _owner.StopAllCoroutines();
        }

        private IEnumerator LongRangeBattleRoutine()
        {
            while (true)
            {
                _isAttacking = true;
                yield return _owner.OnAbility();
                _isAttacking = false;

                // 確率でターゲットに近づく
                if (Random.Range(0, 2) == 1)
                {
                    yield return _owner.OnChase();
                }
                
                // Tickによるステート遷移条件の監視を保証する
                yield return new WaitForSeconds(TICK_INTERVAL_SEC);
            }
        }
    }
}