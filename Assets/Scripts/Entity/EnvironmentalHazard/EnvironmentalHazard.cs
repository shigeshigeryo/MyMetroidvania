using UnityEngine;

public class EnvironmentalHazard : MonoBehaviour
{
    [SerializeField, Tooltip("É_ÉÅÅ[ÉWó ")] private int _attackPower;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.TryGetComponent<StatusManager>(out var damageable))
        {
            damageable.TakeDamage(_attackPower);
        }
    }
}
