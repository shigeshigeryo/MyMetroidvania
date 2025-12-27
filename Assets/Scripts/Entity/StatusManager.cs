using System;
using System.Collections;
using UnityEngine;

public class StatusManager : MonoBehaviour, IDamageable
{
    [SerializeField, Tooltip("ステータスの初期値")]
    private Status _defaultStatus = null;
    // 現在のステータス
    private Status _currentStatus = null;
    public Status CurrentStatus => _currentStatus;
    private bool _isInvincible = false;

    public event Action OnDamaged;
    public event Action OnDead;

    void Start()
    {
        InitializeStatus();
    }

    public void Heal()
    {
        int healValue = _defaultStatus.Life - _currentStatus.Life;
        _currentStatus.UpdateLife(healValue);
        Debug.Log($"回復:{healValue}");
    }

    public void TakeDamage(int damage)
    {
        if (_isInvincible) return;
        _currentStatus.UpdateLife(-damage); //ダメージなので負の数で計算
        if (_currentStatus.Life <= 0)
        {
            OnDead?.Invoke();
        }
        else
        {
            OnDamaged?.Invoke();
            StartCoroutine(OnTakeDamage());
        }
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
    /// 現在のステータス用にインスタンス化
    /// </summary>
    public void InitializeStatus()
    {
        _currentStatus = _defaultStatus.CreateCurrentStatus();
    }
}
