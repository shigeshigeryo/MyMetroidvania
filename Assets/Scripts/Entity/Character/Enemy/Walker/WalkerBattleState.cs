using System.Collections;
using UnityEngine;

namespace MyMetroidVania.Entity.Character.Enemy.Walker
{
    /// <summary>
    /// ウォーカーのバトルステートを管理
    /// </summary>
    public class WalkerBattleState : EnemyState<EnemyWalker>
    {
        private bool _isAttacking = false;
        public WalkerBattleState(EnemyWalker enemy) : base(enemy) { }

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

        /// <summary>
        /// ステートに遷移時に発火
        /// </summary>
        public override void Enter()
        {
            _isAttacking = false;
            _owner.StartCoroutine(BattleRoutine());
        }

        /// <summary>
        /// ステートの状態遷移を監視
        /// </summary>
        protected override void OnTick()
        {
            Debug.Log("ウォーカーのバトルステート行動中");

            if (!_isAttacking && _owner.IsStun)
            {
                // ダメージを受けてスタンした
                // 攻撃中はスタンしない
                _owner.ChangeState(StunState);
                return;
            }
            else if (_owner.IsStun)
            {
                // スタンしないためスタン状態を回復する
                _owner.RecoverStun();
            }

            if (!_isAttacking && !_owner.IsPlayerInRange())
            {
                // 攻撃中にステートの変更を行わない
                // プレイヤーが攻撃射程内にいない
                _owner.ChangeState(ChaseState);
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

        private IEnumerator BattleRoutine()
        {
            while (true)
            {
                _isAttacking = true;
                yield return _owner.OnAttack();
                _isAttacking = false;

                // Tickによるステート遷移条件の監視を保証する
                yield return new WaitForSeconds(TICK_INTERVAL_SEC);
            }
        }
    }
}