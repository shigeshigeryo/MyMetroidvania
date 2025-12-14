using System.Linq;
using UnityEngine;

/// <summary>
/// 指定した判定対象のコライダーと交差しているかを判定する機能を提供します。
/// </summary>
public class BoxCaster : MonoBehaviour
{
    [SerializeField]
    [Tooltip("ボックスの中心座標のオフセットを指定")]
    private Vector2 _offset = new(0, 0);
    [SerializeField]
    [Tooltip("ボックスのサイズを指定")]
    private Vector2 _size = new(1, 1);
    [SerializeField]
    [Tooltip("判定対象のレイヤーを指定")]
    private LayerMask _targetLayers = default;

    /// <summary>
    /// 判定対象と交差している場合はtrue、交差していない場合はfalse
    /// </summary>
    public bool IsCasted { get; private set; } = false;

    void FixedUpdate()
    {
        // 交差判定用のポイントを指定
        var point = transform.TransformPoint(_offset);
        var size = this._size;
        size.x *= transform.lossyScale.x;
        size.y *= transform.lossyScale.y;
        // 交差を判定
        IsCasted = Physics2D.OverlapBox(point, size, transform.eulerAngles.z, _targetLayers);
    }

    /// <summary>
    /// 検知したオブジェクトを返す
    /// </summary>
    public bool TryGetClosestCollider(out Collider2D result)
    {
        Vector2 center = transform.TransformPoint(_offset);
        //Vector2 halfExtents = Vector3.Scale(_range * 0.5f, transform.lossyScale); // *0.5f 半分の大きさを指定する必要があるため

        Collider2D[] colliders = Physics2D.OverlapBoxAll(center, _size, 0, _targetLayers);
        
        // プレイヤーとの距離が近いcolliderを取得
        result = colliders.OrderBy(c => (c.transform.position - transform.position).sqrMagnitude)
                         .First();
        
        return result != null;
    }

    private void OnDrawGizmos()
    {
        // 交差判定用のポイントを設定
        var halfSize = _size / 2;
        var controlPoints = new Vector3[]
        {
        _offset + new Vector2(-halfSize.x, +halfSize.y), // 左上
        _offset + new Vector2(+halfSize.x, +halfSize.y), // 右上
        _offset + new Vector2(+halfSize.x, -halfSize.y), // 右下
        _offset + new Vector2(-halfSize.x, -halfSize.y), // 左下
        };
        transform.TransformPoints(controlPoints);
        // 判定ラインを描画
        Gizmos.color = Color.yellow;
        Gizmos.DrawLineStrip(controlPoints, true);
    }
}
