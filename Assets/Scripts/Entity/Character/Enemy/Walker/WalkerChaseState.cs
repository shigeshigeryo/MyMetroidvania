using System.Collections;
using UnityEngine;

namespace MyMetroidVania.Entity.Character.Enemy.Walker
{
    /// <summary>
    /// ウォーカーの追跡ステートを管理
    /// </summary>
    public class WalkerChaseState : EnemyState<EnemyWalker>
    {
        public WalkerChaseState(EnemyWalker enemy) : base(enemy) { }

        /// <summary>
        /// ステートに遷移時に発火
        /// </summary>
        public override void Enter()
        {
            routine = _owner.StartCoroutine(ChaseRoutine());
        }

        /// <summary>
        /// ステートの状態遷移を監視
        /// </summary>
        protected override void OnTick()
        {
            Debug.Log("ウォーカーの追跡ステート行動中");
            if (!_owner.IsPlayerDetected())
            {
                // プレイヤーが検知外に出た
                _owner.ChangeState(new WalkerIdleState(_owner));
                return;
            }

            if (_owner.IsPlayerInRange())
            {
                // プレイヤーが攻撃射程内にいる
                _owner.ChangeState(new WalkerBattleState(_owner));
                return;
            }
        }

        /// <summary>
        /// 次のステートに遷移する前に発火
        /// </summary>
        public override void Exit()
        {
            _owner.StopCoroutine(routine);
            _owner.StopChase();
        }

        private IEnumerator ChaseRoutine()
        {
            while (true)
            {
                yield return _owner.OnChase();
            }
        }
    }
}