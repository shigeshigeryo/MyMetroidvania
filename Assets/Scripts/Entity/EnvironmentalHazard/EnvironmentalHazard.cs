using UnityEngine;

namespace MyMetroidVania.Entity.EnvironmentalHazard
{
    public class EnvironmentalHazard : MonoBehaviour, IDamageDealer
    {
        [SerializeField, Tooltip("É_ÉÅÅ[ÉWó ")] private int _attackPower;

        public int GetAttackPower()
        {
            return _attackPower;
        }
    }
}