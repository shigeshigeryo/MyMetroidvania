using UnityEngine;
using UnityEngine.Pool;

namespace MyMetroidVania.Entity.Effect
{
    public class PoolingEffect : MonoBehaviour
    {
        private IObjectPool<PoolingEffect> _pool;

        public void SetPool(IObjectPool<PoolingEffect> pool)
        {
            _pool = pool;
        }

        /// <summary>
        /// アニメーションが終わったら発火する　※アニメーションイベントで設定
        /// プールに戻す処理
        /// </summary>
        public void OnFinished()
        {
            _pool.Release(this);
        }
    }
}