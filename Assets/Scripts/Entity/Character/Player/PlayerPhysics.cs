using UnityEngine;

namespace MyMetroidVania.Entity.Character.Player
{
    public class PlayerPhysics : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rb = null;

        [Header("移動")]
        [SerializeField, Tooltip("x軸の移動の速さ")] private float _moveSpeedX = 5f;
        [SerializeField, Tooltip("Walk中に移動速度を超えたときに抵抗としてかかる毎秒の速度")]
        private float _deceleration = 10f;

        [Header("ジャンプ")]
        [SerializeField, Tooltip("ジャンプの初速")] private float _jumpSpeed = 8f;
        [SerializeField, Tooltip("ジャンプボタン押下時にかかる+yの加速度")] private float _jumpAccel = 10f;
        [SerializeField, Tooltip("落下速上限")] private float _maxFallingSpeed = 13f;
        public Vector2 Velocity => _rb.linearVelocity;
        public bool IsMoving => Mathf.Abs(_rb.linearVelocityX) > 0.01f;
        public bool IsFalling => _rb.linearVelocityY < -0.01f;


        /*
         * ------------------------------------------------------------------
         * 移動挙動
         * ------------------------------------------------------------------
         */
        /// <summary>
        /// 移動速度をセットする
        /// フック後で速度が出ている場合はそのままの速度を保たせる
        /// TODO：フック後に移動していないと不自然に止まるので、直すかどうか検討
        /// </summary>
        /// <param name="dirX"></param>
        public void SetMoveVelocity(float dirX)
        {
            if (Mathf.Abs(_moveSpeedX * dirX) > Mathf.Abs(_rb.linearVelocityX) // 入力値が現在の早さを上回るか
                || Mathf.Sign(dirX) != Mathf.Sign(_rb.linearVelocityX) // 速度方向は一致していないか
                || Mathf.Abs(dirX) < 0.01f) // x軸の入力が0付近か
            {
                _rb.linearVelocityX = _moveSpeedX * dirX;
            }
        }

        /// <summary>
        /// 現在の速さが規定の移動速を超えていた場合に徐々に速さを減らす
        /// </summary>
        public void ReduceExcessSpeed()
        {
            if (Mathf.Abs(_rb.linearVelocityX) > _moveSpeedX)
            {
                float flg = _rb.linearVelocityX >= 0 ? -1 : 1;
                _rb.linearVelocityX += flg * _deceleration * Time.fixedDeltaTime;
            }
        }

        /// <summary>
        /// 落下速度を抑制する
        /// </summary>
        public void RestrainFallingSpeed()
        {
            _rb.linearVelocityY = Mathf.Max(_rb.linearVelocityY, -_maxFallingSpeed);
        }

        /*
         * ------------------------------------------------------------------
         * ジャンプ挙動
         * ------------------------------------------------------------------
         */
        /// <summary>
        /// ジャンプする
        /// </summary>
        public void Jump()
        {
            // ジャンプ速度設定
            var newVelocity = _rb.linearVelocity;
            newVelocity.y = _jumpSpeed;
            _rb.linearVelocity = newVelocity;
        }

        /// <summary>
        /// ジャンプ中にジャンプボタンを押している時にかかる上向き正の加速
        /// </summary>
        public void AccelerateJump()
        {
            var tmpVelocity = _rb.linearVelocity;
            // 落下し始めたタイミングから加速を切る
            if (tmpVelocity.y < 0) return;

            tmpVelocity.y += _jumpAccel * Time.fixedDeltaTime;
            _rb.linearVelocity = tmpVelocity;
        }


        /*
         * ------------------------------------------------------------------
         * 特殊挙動（アビリティなどを用いた場合）
         * ------------------------------------------------------------------
         */
        /// <summary>
        /// 速度の設定
        /// </summary>
        /// <param name="vel">velocityを指定</param>
        public void SetVelocity(Vector2 vel)
        {
            _rb.linearVelocity = vel;
        }
    }
}