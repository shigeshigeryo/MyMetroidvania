using System;
using UnityEngine;

namespace MyMetroidVania.Entity
{
    /// <summary>
    /// 攻撃判定が与えるダメージをヒット側に通知する
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class HitBox : MonoBehaviour
    {
        private IDamageDealer _dealer = null; // Nullの場合は damageValue の値をダメージとして与える
        [SerializeField, Tooltip("ステータスを参照しない場合のダメージ")]
        private int _damageValue = 1;
        [SerializeField, Tooltip("ステータスを参照する場合のダメージ倍率")]
        private float _attackMul = 1.0f;
        private Collider2D _collider;
        /// <summary>
        /// 攻撃判定がトリガーしたタイミングで発火するイベント
        /// </summary>
        public event Action OnTriggered;

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Awake()
        {
            _dealer = GetComponentInParent<IDamageDealer>();
            _collider = GetComponent<Collider2D>();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="dealer">攻撃者</param>
        public void Initialize(IDamageDealer dealer)
        {
            _dealer = dealer;
        }

        /// <summary>
        /// 与えるダメージを取得する
        /// </summary>
        /// <returns>与えるダメージの値</returns>
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

        /// <summary>
        /// 攻撃判定をONにする
        /// </summary>
        public void SetEnableCollider()
        {
            _collider.enabled = true;
        }

        /// <summary>
        /// 攻撃判定をOFFにする
        /// </summary>
        public void SetDisableCollider()
        {
            _collider.enabled = false;
        }

        /// <summary>
        /// 親オブジェクトを考慮したPositionにセット
        /// </summary>
        /// <param name="newVec"></param>
        public void SetPosition(Vector3 newVec)
        {
            transform.localPosition = newVec;
        }

        /// <summary>
        /// トリガー判定
        /// ダメージ処理を行う
        /// </summary>
        /// <param name="collision">トリガー対象</param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent<HurtBox>(out var hurtBox))
            {
                hurtBox.TakeDamage(GetDamageValue());
                OnTriggered?.Invoke();
            }
        }
    }
}
