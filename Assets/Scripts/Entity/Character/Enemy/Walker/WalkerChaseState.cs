using System.Collections;

namespace MyMetroidVania.Entity.Character.Enemy.Walker
{
    /// <summary>
    /// ウォーカーの追跡ステートを管理
    /// </summary>
    public class WalkerChaseState : EnemyState<EnemyWalker>
    {
        public WalkerChaseState(EnemyWalker enemy) : base(enemy) { }

        private WalkerIdleState _idleState = null;
        private WalkerIdleState IdleState
        {
            get
            {
                if (_idleState == null)
                {
                    // 存在しなかった場合は生成して返す
                    return _idleState = new WalkerIdleState(_owner);
                }
                else
                {
                    return _idleState;
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

        /// <summary>
        /// 追跡時の挙動を管理
        /// </summary>
        private IEnumerator ChaseRoutine()
        {
            while (true)
            {
                yield return _owner.OnChase();
            }
        }
    }
}
