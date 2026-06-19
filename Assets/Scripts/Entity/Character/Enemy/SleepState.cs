namespace MyMetroidVania.Entity.Character.Enemy
{
    /// <summary>
    /// 全エネミー共通のスリープ状態のステートを管理
    /// </summary>
    public class SleepState : EnemyState<EnemyBase>
    {
        public EnemyState NextState; // スリープ解除時に遷移するステート
        public SleepState(EnemyBase enemy, EnemyState nextState) : base(enemy)
        {
            NextState = nextState;
        }

        /// <summary>
        /// ステートの状態遷移を監視
        /// </summary>
        protected override void OnTick()
        {
            if (_owner.IsVisible())
            {
                // 画面内に映っている場合にステートを変更
                _owner.ChangeState(NextState);
            }
        }
    }
}
