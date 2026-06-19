using UnityEngine;
using UnityEngine.Pool;

namespace MyMetroidVania.Entity.Effect
{
    /// <summary>
    /// プーリングするエフェクトの基底クラス
    /// </summary>
    public class PoolingEffect : MonoBehaviour
    {
        private IObjectPool<PoolingEffect> _pool;

        /// <summary>
        /// プールを設定する
        /// </summary>
        /// <param name="pool">設定するプール</param>
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
