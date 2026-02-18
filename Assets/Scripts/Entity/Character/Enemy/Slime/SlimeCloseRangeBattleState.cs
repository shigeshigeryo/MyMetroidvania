using System.Collections;
using UnityEngine;

namespace MyMetroidVania.Entity.Character.Enemy.Slime
{
    public class SlimeCloseRangeBattleState : EnemyState<EnemySlime>
    {
        private bool _isAttacking = false;

        public SlimeCloseRangeBattleState(EnemySlime enemy) : base(enemy) { }

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

        private SlimeLongRangeBattleState _longRangeBattleState = null;
        private SlimeLongRangeBattleState LongRangeBattleState
        {
            get
            {
                if (_longRangeBattleState == null)
                {
                    // 存在しなかった場合は生成して返す
                    return _longRangeBattleState = new SlimeLongRangeBattleState(_owner);
                }
                else
                {
                    return _longRangeBattleState;
                }
            }
        }

        /// <summary>
        /// ステートに遷移時に発火
        /// </summary>
        public override void Enter()
        {
            _isAttacking = false;
            _owner.StartCoroutine(CloseRangeBattleRoutine());
        }

        /// <summary>
        /// ステートの状態遷移を監視
        /// </summary>
        protected override void OnTick()
        {
            Debug.Log("スライムの近距離バトルステート行動中");

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

            if (!_isAttacking && !_owner.IsPlayerInRange())
            {
                // 攻撃中にステートの変更を行わない
                // プレイヤーが近距離射程内にいない
                _owner.ChangeState(LongRangeBattleState);
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

        private IEnumerator CloseRangeBattleRoutine()
        {
            while (true)
            {
                _isAttacking = true;
                yield return _owner.OnAttack();
                _isAttacking = false;

                // 確率でターゲットに近づくか離れる
                if (Random.Range(0,2) == 1)
                {
                    yield return _owner.OnChase();
                }
                else
                {
                    yield return _owner.OnEscape();
                }

                // Tickによるステート遷移条件の監視を保証する
                yield return new WaitForSeconds(TICK_INTERVAL_SEC);
            }
        }
    }
}