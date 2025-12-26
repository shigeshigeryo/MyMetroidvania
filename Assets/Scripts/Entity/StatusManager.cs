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
        InitializeStatus();
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

    /// <summary>
    /// ステータスの初期化、主にスポーン時に発火
    /// </summary>
    public void InitializeStatus()
    {
        _isDead = false;
        // 現在のステータス用にインスタンス化
        _currentStatus = _defaultStatus.CreateCurrentStatus();
    }
}
