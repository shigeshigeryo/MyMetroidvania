using System.Collections;

namespace MyMetroidVania.Entity.Character.Enemy.Slime
{
    /// <summary>
    /// スライムの待機時のステートを管理
    /// </summary>
    public class SlimeIdleState : EnemyState<EnemySlime>
    {
        public SlimeIdleState(EnemySlime enemy) : base(enemy) { }

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

        /// <summary>
        /// ステートに遷移時に発火
        /// </summary>
        public override void Enter()
        {
            _owner.StartCoroutine(IdleRoutine());
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

            if (_owner.IsPlayerDetected())
            {
                // プレイヤーを検知した
                _owner.ChangeState(LongRangeBattleState);
                return;
            }

            if (!_owner.IsVisible())
            {
                // 画面外に出た
                _owner.ChangeState(SleepState);
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
            _owner.StopMove();
        }

        /// <summary>
        /// 待機時の挙動を管理
        /// </summary>
        private IEnumerator IdleRoutine()
        {
            yield return _owner.OnIdle();
        }
    }
}
