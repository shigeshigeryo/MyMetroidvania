using MyMetroidVania.Utility;
using UnityEngine;
using UnityEngine.Pool;

namespace MyMetroidVania.Entity.Character.Enemy.Slime
{
    /// <summary>
    /// スライムから出るボールを管理
    /// </summary>
    public class SlimeBall : MonoBehaviour
    {
        [SerializeField] private HitBox _hitBox = null;
        [SerializeField] private CircleCaster _groundChecker = null;
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

        /// <summary>
        /// プールを設定する
        /// </summary>
        /// <param name="pool">設定するプール</param>
        public void SetPool(IObjectPool<SlimeBall> pool)
        {
            _pool = pool;
        }

        /// <summary>
        /// 接地判定でボールが消える処理
        /// </summary>
        private void FixedUpdate()
        {
            // 接地判定でリリース
            if(_groundChecker.IsCasted)
            {
                _pool.Release(this);
            }
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
