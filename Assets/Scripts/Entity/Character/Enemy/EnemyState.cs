using UnityEngine;

namespace MyMetroidVania.Entity.Character.Enemy 
{
    /// <summary>
    /// エネミーの状態を管理する基底クラス
    /// </summary>
    public abstract class EnemyState
    {
        protected float _timer = 0;
        protected const float TICK_INTERVAL_SEC = 0.1f; // Tickインターバル秒

        /// <summary>
        /// ステートに遷移時に発火
        /// </summary>
        public virtual void Enter() { }
        /// <summary>
        /// ステートの監視を制御
        /// Update内で発火
        /// </summary>
        public void Tick()
        {
            // 遷移の監視にインターバルを設ける
            if (_timer < TICK_INTERVAL_SEC)
            {
                _timer += Time.deltaTime;
                return;
            }
            _timer = 0; // タイマーリセット

            OnTick();
        }
        /// <summary>
        /// 毎インターバル時に発火する処理
        /// </summary>
        protected abstract void OnTick();
        /// <summary>
        /// 次のステートに遷移する前に発火
        /// </summary>
        public virtual void Exit() { }
    }

    /// <summary>
    /// _ownerの型情報を具体化するために実装
    /// </summary>
    /// <typeparam name="T">EnemyBaseの派生クラス</typeparam>
    public abstract class EnemyState<T> : EnemyState where T : EnemyBase
    {
        protected T _owner;
        private StunState _stunState = null;
        protected StunState StunState
        {
            get
            {
                if (_stunState == null)
                {
                    // 存在しなかった場合は生成して返す
                    return _stunState = new StunState(_owner, this);
                }
                else
                {
                    return _stunState;
                }
            }
        }

        public EnemyState(T enemy)
        {
            _owner = enemy;
        }
    }
}
