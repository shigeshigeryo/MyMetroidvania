using UnityEngine;

/// <summary>
/// 指定した判定対象のコライダーと交差しているかを判定する機能を提供します。
/// </summary>
public class BoxCaster : MonoBehaviour
{
    [SerializeField]
    [Tooltip("ボックスの中心座標のオフセットを指定")]
    private Vector2 offset = new(0, 0);
    [SerializeField]
    [Tooltip("ボックスのサイズを指定")]
    private Vector2 size = new(1, 1);
    [SerializeField]
    [Tooltip("判定対象のレイヤーを指定")]
    private LayerMask targetLayers = default;

    /// <summary>
    /// 判定対象と交差している場合はtrue、交差していない場合はfalse
    /// </summary>
    public bool IsCasted { get; private set; } = false;

    void FixedUpdate()
    {
        // 交差判定用のポイントを指定
        var point = transform.TransformPoint(offset);
        var size = this.size;
        size.x *= transform.lossyScale.x;
        size.y *= transform.lossyScale.y;
        // 交差を判定
        IsCasted = Physics2D.OverlapBox(point, size, transform.eulerAngles.z, targetLayers);
    }

    private void OnDrawGizmos()
    {
        // 交差判定用のポイントを設定
        var halfSize = size / 2;
        var controlPoints = new Vector3[]
        {
        offset + new Vector2(-halfSize.x, +halfSize.y), // 左上
        offset + new Vector2(+halfSize.x, +halfSize.y), // 右上
        offset + new Vector2(+halfSize.x, -halfSize.y), // 右下
        offset + new Vector2(-halfSize.x, -halfSize.y), // 左下
        };
        transform.TransformPoints(controlPoints);
        // 判定ラインを描画
        Gizmos.color = Color.yellow;
        Gizmos.DrawLineStrip(controlPoints, true);
    }
}
