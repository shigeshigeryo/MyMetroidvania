using UnityEngine;

/// <summary>
/// 攻撃判定が与えるダメージをヒット側に通知する
/// </summary>
public class DamageSource : MonoBehaviour
{
    private IDamageDealer _dealer = null; // Nullの場合は damageValue の値をダメージとして与える
    [SerializeField, Tooltip("ステータスを参照しない場合のダメージ")]
    private int _damageValue = 1;
    [SerializeField, Tooltip("ステータスを参照する場合のダメージ倍率")]
    private float _attackMul = 1.0f;

    private void Start()
    {
        _dealer = GetComponentInParent<IDamageDealer>();
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<HitBox>(out var hitBox))
        {
            hitBox.TakeDamage(GetDamageValue());
        }
    }
}
