using UnityEngine;

namespace MyMetroidVania.Entity
{
    /// <summary>
    /// 被弾側の通知を管理
    /// </summary>
    public class HurtBox : MonoBehaviour
    {
        [SerializeField] private StatusManager _statusManager = null;

        /// <summary>
        /// ダメージを受ける処理
        /// </summary>
        /// <param name="damage">受けるダメージ量</param>
        public void TakeDamage(int damage)
        {
            _statusManager.TakeDamage(damage);
        }
    }
}
