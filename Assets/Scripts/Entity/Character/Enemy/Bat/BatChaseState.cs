using System.Collections;

namespace MyMetroidVania.Entity.Character.Enemy.Bat
{
    public class BatChaseState : EnemyState<EnemyBat>
    {
        public BatChaseState(EnemyBat enemy) : base(enemy) { }

        private BatIdleState _idleState = null;
        private BatIdleState IdleState
        {
            get
            {
                if (_idleState == null)
                {
                    // 存在しなかった場合は生成して返す
                    return _idleState = new BatIdleState(_owner);
                }
                else
                {
                    return _idleState;
                }
            }
        }
        private BatBattleState _battleState = null;
        private BatBattleState BattleState
        {
            get
            {
                if (_battleState == null)
                {
                    // 存在しなかった場合は生成して返す
                    return _battleState = new BatBattleState(_owner);
                }
                else
                {
                    return _battleState;
                }
            }
        }

        /// <summary>
        /// ステートに遷移時に発火
        /// </summary>
        public override void Enter()
        {
            _owner.StartChase();
            _owner.StartCoroutine(ChaseRoutine());
        }

        /// <summary>
        /// ステートの状態遷移を監視
        /// </summary>
        protected override void OnTick()
        {
            if (_owner.IsStun)
            {
                // ダメージを受けてスタンした
                StunState.SetNextState(this);
                _owner.ChangeState(StunState);
                return;
            }

            if (!_owner.IsPlayerDetected())
            {
                // プレイヤーが検知外に出た
                _owner.ChangeState(IdleState);
                return;
            }

            if (_owner.IsPlayerInRange())
            {
                // プレイヤーが攻撃射程内にいる
                _owner.ChangeState(BattleState);
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