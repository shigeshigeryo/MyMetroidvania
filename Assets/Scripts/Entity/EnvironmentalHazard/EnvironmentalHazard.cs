using UnityEngine;

namespace MyMetroidVania.Entity.EnvironmentalHazard
{
    /// <summary>
    /// 接触ダメージの環境物
    /// </summary>
    public class EnvironmentalHazard : MonoBehaviour, IDamageDealer
    {
        [SerializeField, Tooltip("ダメージ量")] private int _attackPower;

        /// <summary>
        /// 攻撃力を取得する
        /// </summary>
        /// <returns>攻撃力の値</returns>
        public int GetAttackPower()
        {
            return _attackPower;
        }
    }
}
