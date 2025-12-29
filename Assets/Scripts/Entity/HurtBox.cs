using UnityEngine;

public class HurtBox : MonoBehaviour
{
    [SerializeField] private StatusManager _statusManager = null;

    public void TakeDamage(int damage)
    {
        _statusManager.TakeDamage(damage);
    }
}
