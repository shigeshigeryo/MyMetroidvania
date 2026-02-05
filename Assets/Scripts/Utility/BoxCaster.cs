using System.Linq;
using UnityEngine;

namespace MyMetroidVania.Utility
{
    /// <summary>
    /// 指定した判定対象のコライダーと交差しているかを判定する機能を提供します。
    /// </summary>
    public class BoxCaster : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("判定対象のレイヤーを指定")]
        private LayerMask _targetLayers = default;

        // TODO：カスタムでフラグの値によって表示、非表示を切り替えられると良い
        [Header("Overlap")]
        [SerializeField]
        [Tooltip("OverlapBoxを使用するか")]
        private bool _useOverlapBox = true;
        [SerializeField]
        [Tooltip("ボックスの中心座標のオフセットを指定")]
        private Vector2 _offsetOverlap = new(0, 0);
        [SerializeField]
        [Tooltip("ボックスのサイズを指定")]
        private Vector2 _sizeOverlap = new(1, 1);
        [SerializeField]
        [Tooltip("ギズモの色を指定")]
        private Color _gizmoColorOverlap = Color.white;

        [Header("BoxCast")]
        [SerializeField]
        [Tooltip("BoxCastを使用するか")]
        private bool _useBoxCast = true;
        [SerializeField]
        [Tooltip("ボックスの中心座標のオフセットを指定")]
        private Vector2 _offsetCast = new(0, 0);
        [SerializeField]
        [Tooltip("ボックスのサイズを指定")]
        private Vector2 _sizeCast = new(1, 1);
        [SerializeField]
        [Tooltip("キャスト距離を指定")]
        private float _distanceCast = 5f;
        [SerializeField]
        [Tooltip("ギズモの色を指定")]
        private Color _gizmoColorCast = Color.blue;

        /// <summary>
        /// 判定対象と交差している場合はtrue、交差していない場合はfalse
        /// </summary>
        public bool IsCasted { get; private set; } = false;

        void FixedUpdate()
        {
            if (_useOverlapBox)
            {
                // 交差判定用のポイントを指定
                var point = transform.TransformPoint(_offsetOverlap);
                var size = _sizeOverlap * transform.lossyScale;
                // 交差を判定
                IsCasted = Physics2D.OverlapBox(point, size, transform.eulerAngles.z, _targetLayers);
            }
        }

        /// <summary>
        /// 検知したオブジェクトを返す
        /// </summary>
        public bool TryGetClosestCollider(out Collider2D result)
        {
            Vector2 center = transform.TransformPoint(_offsetOverlap);
            var size = _sizeOverlap * transform.lossyScale;
            Collider2D[] colliders = Physics2D.OverlapBoxAll(center, size, transform.eulerAngles.z, _targetLayers);

            if (colliders.Length == 0)
            {
                result = null;
                return false;
            }

            // プレイヤーとの距離が近いcolliderを取得
            result = colliders.OrderBy(c => (c.transform.position - transform.position).sqrMagnitude)
                              .FirstOrDefault();

            return result != null;
        }

        /// <summary>
        /// 検知したオブジェクトを返すBoxCas\t
        /// </summary>
        public RaycastHit2D GetBoxCast()
        {
            var dir = transform.right;
            Vector2 center = (Vector2)transform.TransformPoint(_offsetCast);
            var size = _sizeCast * transform.lossyScale;

            return Physics2D.BoxCast(center,
                                     size,
                                     transform.eulerAngles.z,
                                     dir,
                                     _distanceCast,
                                     _targetLayers);
        }

        private void OnEnable()
        {
            // Inactive時に判定結果の初期化をしておく
            IsCasted = false;
        }
        private void OnDrawGizmos()
        {
            var angle = transform.eulerAngles.z;
            // OverlapBoxのギズモ
            if (_useOverlapBox)
            {
                var sizeOverlap = _sizeOverlap * transform.lossyScale;
                DrawBox((Vector2)transform.TransformPoint(_offsetOverlap), sizeOverlap, angle, _gizmoColorOverlap);
            }

            // BoxCastのギズモ
            // BoxCastは中心点までを参照するので、外側半分に当たり判定がないことに注意
            if (_useBoxCast)
            {
                var dir = transform.right;
                var size = _sizeCast * transform.lossyScale;
                Vector2 center = (Vector2)transform.TransformPoint(_offsetCast);
                Vector2 endCenter = center + (Vector2)dir * _distanceCast;

                DrawBox(center, size, angle, _gizmoColorCast);
                DrawBox(endCenter, size, angle, _gizmoColorCast);
                Gizmos.DrawLine(center, endCenter);
            }
        }

        /// <summary>
        /// ボックスのギズモを生成
        /// 中心座標はワールド準拠で渡す
        /// </summary>
        /// <param name="center"></param>
        /// <param name="size"></param>
        /// <param name="angle" ></param>
        /// <param name="color"></param>
        private void DrawBox(Vector2 center, Vector2 size, float angle, Color color)
        {
            // 交差判定用のポイントを設定
            var halfSize = size / 2;
            var angelAxis = Quaternion.AngleAxis(angle, Vector3.forward);
            var a = angelAxis * new Vector3(-halfSize.x, +halfSize.y, 0);
            var controlPoints = new Vector3[]
            {
                new Vector2(-halfSize.x, +halfSize.y), // 左上
                new Vector2(+halfSize.x, +halfSize.y), // 右上
                new Vector2(+halfSize.x, -halfSize.y), // 右下
                new Vector2(-halfSize.x, -halfSize.y), // 左下
            };

            for (int i = 0; i < controlPoints.Length; i++)
            {
                controlPoints[i] = (angelAxis * controlPoints[i]) + (Vector3)center;
            }

            // 判定ラインを描画
            Gizmos.color = color;
            Gizmos.DrawLineStrip(controlPoints, true);
        }
    }
}
