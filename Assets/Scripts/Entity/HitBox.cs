using UnityEngine;

/// <summary>
/// 攻撃判定が与えるダメージをヒット側に通知する
/// </summary>
[RequireComponent (typeof(Collider2D))]
public class HitBox : MonoBehaviour
{
    private IDamageDealer _dealer = null; // Nullの場合は damageValue の値をダメージとして与える
    [SerializeField, Tooltip("ステータスを参照しない場合のダメージ")]
    private int _damageValue = 1;
    [SerializeField, Tooltip("ステータスを参照する場合のダメージ倍率")]
    private float _attackMul = 1.0f;
    private Collider2D _collider;

    private void Start()
    {
        _dealer = GetComponentInParent<IDamageDealer>();
        _collider = GetComponent<Collider2D>();
    }

    private int GetDamageValue()
    {
        if (_dealer == null)
        {
            return _damageValue;
        }
        else
        {
            return Mathf.FloorToInt(_dealer.GetAttackPower() * _attackMul);
        }
    }

    public void SetEnableCollider()
    {
        _collider.enabled = true;
    }

    public void SetDisableCollider()
    {
        _collider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<HurtBox>(out var hurtBox))
        {
            hurtBox.TakeDamage(GetDamageValue());
        }
    }
}
