using UnityEngine;

public class StatusManager : MonoBehaviour
{
    [SerializeField, Tooltip("ステータスの初期値")]
    private Status _defaultStatus = null;
    // 現在のステータス
    private Status _currentStatus = null;
    public Status CurrentStatus => _currentStatus;

    void Start()
    {
        _currentStatus = _defaultStatus;
    }
}
