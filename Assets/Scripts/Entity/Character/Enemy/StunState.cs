namespace MyMetroidVania.Entity.Character.Enemy
{
    /// <summary>
    /// 全エネミー共通のスリープ状態のステートを管理
    /// </summary>
    public class StunState : EnemyState<EnemyBase>
    {
        public EnemyState NextState; // スリープ解除時に遷移するステート
        private const float STUN_TIMER = 0.1f;
        private float _stunTimer = 0f;

        public StunState(EnemyBase enemy, EnemyState nextState) : base(enemy)
        {
            NextState = nextState;
        }

        /// <summary>
        /// ステートに遷移時に発火
        /// </summary>
        public override void Enter()
        {
            _stunTimer = STUN_TIMER;
            _owner.StopMove(); // 移動速度が与えられていた場合は止める
        }

        /// <summary>
        /// ステートの状態遷移を監視
        /// </summary>
        protected override void OnTick()
        {
            if (_stunTimer <= 0f)
            {
                // スタン時間が過ぎたらステートを変更
                _owner.ChangeState(NextState);
            }

            _stunTimer -= TICK_INTERVAL_SEC;
        }

        /// <summary>
        /// 次のステートに遷移する前に発火
        /// </summary>
        public override void Exit()
        {
            // スタン状態を解除
            _owner.RecoverStun();
        }

        public void SetNextState(EnemyState state)
        {
            NextState = state;
        }
    }
}