using System.Collections;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    [SerializeField, Tooltip("ステータスの初期値")]
    private Status _defaultStatus = null;
    // 現在のステータス
    private Status _currentStatus = null;
    public Status CurrentStatus => _currentStatus;
    private bool _isInvincible = false;
    private bool _isDead = false;
    public bool IsDead => _isDead;

    void Start()
    {
        // 現在のステータス用にインスタンス化
        _currentStatus = _defaultStatus.CreateCurrentStatus();
    }

    public void TakeDamage(int damage)
    {
        if (_isInvincible) return;

        _currentStatus.UpdateLife(-damage); //ダメージなので負の数で計算
        if (_currentStatus.Life <= 0)
        {
            _isDead = true;
            return;
        }
        StartCoroutine(OnTakeDamage());
        Debug.Log($"life:{_currentStatus.Life}");
        Debug.Log($"attackPower:{_currentStatus.AttackPower}");
    }

    /// <summary>
    /// ダメージ発生後の無敵化
    /// </summary>
    private IEnumerator OnTakeDamage()
    {
        _isInvincible = true;
        yield return new WaitForSeconds(_currentStatus.InvincibleSec);
        _isInvincible = false;
    }
}
