using UnityEngine;
using UnityEngine.Pool;

namespace MyMetroidVania.Entity.Character.Enemy.Slime
{
    public class SlimeBall : MonoBehaviour
    {
        [SerializeField] private HitBox _hitBox = null;
        [SerializeField] private Rigidbody2D _rb = null;

        private IObjectPool<SlimeBall> _pool;

        /// <summary>
        /// 初期化処理
        /// ヒットボックスにダメージディーラーの情報を渡す
        /// </summary>
        /// <param name="dealer"></param>
        public void InitializeOnCreate(IDamageDealer dealer)
        {
            _hitBox.Initialize(dealer);
        }

        public void SetPool(IObjectPool<SlimeBall> pool)
        {
            _pool = pool;
        }

        /// <summary>
        /// 放物線を描くような力を加える
        /// </summary>
        /// <param name="force">x軸方向の力</param>
        public void AddForceParabolicTrajectory(float force)
        {
            Vector2 val = new Vector2(force, 12);
            _rb.AddForce(val, ForceMode2D.Impulse);
        }

        /// <summary>
        /// なにかと接触したタイミングでリリースする
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            _pool.Release(this);
        }
    }
}