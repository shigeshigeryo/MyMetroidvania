using UnityEngine;

namespace MyMetroidVania.Utility
{
    // 指定した判定対象のコライダーと交差しているかを判定する機能を提供します。
    public class LineCaster : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("ラインの開始座標のオフセットを指定")]
        private Vector3 _startOffset = new(0, 0.25f, 0);
        [SerializeField]
        [Tooltip("ラインの終了点のオフセットを指定")]
        private Vector3 _endOffset = new(0, -0.25f, 0);
        [SerializeField]
        [Tooltip("判定対象のレイヤーを指定")]
        private LayerMask _targetLayers = default;
        [SerializeField]
        [Tooltip("ギズモの色を指定")]
        private Color _gizmoColor = Color.white;

        // 判定対象と交差している場合はtrue、交差していない場合はfalse
        public bool IsCasted { get; private set; } = false;

        // 固定フレームレートで呼び出される更新処理です。
        void FixedUpdate()
        {
            // 交差判定用の座標を設定
            var start = transform.TransformPoint(_startOffset);
            var end = transform.TransformPoint(_endOffset);

            // 交差を判定
            IsCasted = Physics2D.Linecast(start, end, _targetLayers);
        }

        public RaycastHit2D GetRaycastHit()
        {
            var start = transform.TransformPoint(_startOffset);
            var end = transform.TransformPoint(_endOffset);
            var dir = end - start;
            return Physics2D.Raycast(start, dir.normalized, dir.magnitude, _targetLayers);
        }

        private void OnEnable()
        {
            // Inactive時に判定結果の初期化をしておく
            IsCasted = false;
        }

        // Unityエディター上で常時描画するギズモを記述します。
        private void OnDrawGizmos()
        {
            // 交差判定用のポイントを設定
            var start = transform.TransformPoint(_startOffset);
            var end = transform.TransformPoint(_endOffset);
            // 判定ラインを描画
            Gizmos.color = _gizmoColor;
            Gizmos.DrawLine(start, end);
        }
    }
}