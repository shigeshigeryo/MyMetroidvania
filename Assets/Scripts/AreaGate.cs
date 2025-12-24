using UnityEngine;

public class AreaGate : MonoBehaviour
{
    [SerializeField, Tooltip("移動先エリアのID")] private string _nextAreaId;
    public string NextAreaId => _nextAreaId;
    [SerializeField, Tooltip("移動先エリアのプレイヤーのスポーン場所")] private Transform _spawnPosition;
    public Transform SpawnPosition => _spawnPosition;
}
