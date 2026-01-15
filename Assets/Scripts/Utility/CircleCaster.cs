using UnityEngine;

namespace MyMetroidVania.Utility
{
    /// <summary>
    /// 円判定を行う
    /// </summary>
    public class CircleCaster : MonoBehaviour
    {
        [SerializeField, Tooltip("中心座標のオフセットを指定")]
        private Vector3 _offset = Vector3.zero;
        [SerializeField, Tooltip("円の半径")]
        private float _radius = 5.0f;
        [SerializeField, Tooltip("判定対象のレイヤーを指定")]
        private LayerMask _targetLayers = default;
        [SerializeField, Tooltip("ギズモの色を指定")]
        private Color _gizmoColor = Color.white;

        public Collider2D GetHitCollider()
        {
            return Physics2D.OverlapCircle(transform.position, _radius, _targetLayers);
        }

        // Unityエディター上で常時描画するギズモを記述します。
        private void OnDrawGizmos()
        {
            // 判定ラインを描画
            Gizmos.color = _gizmoColor;
            Gizmos.DrawWireSphere(transform.position + _offset, _radius);
        }
    }
}