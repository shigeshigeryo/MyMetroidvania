using UnityEngine;
using UnityEngine.Pool;

namespace MyMetroidVania.Entity.Character.Enemy.Slime
{
    public class ShockWave : MonoBehaviour
    {
        [SerializeField] private HitBox _hitBox = null;
        [SerializeField] private SpriteRenderer _renderer = null;

        private IObjectPool<ShockWave> _pool;

        /// <summary>
        /// 初期化処理
        /// ヒットボックスにダメージディーラーの情報を渡す
        /// </summary>
        /// <param name="dealer"></param>
        public void Initialize(IDamageDealer dealer)
        {
            _hitBox.Initialize(dealer);
        }

        public void SetPool(IObjectPool<ShockWave> pool)
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

        /// <summary>
        /// スプライトを反転させる
        /// </summary>
        /// <param name="flip">右側に広がる場合はtrue<br>左側に広がる場合はfalse</param>
        public void SetFlipX(bool flip)
        {
            _renderer.flipX = flip;
        }
    }
}