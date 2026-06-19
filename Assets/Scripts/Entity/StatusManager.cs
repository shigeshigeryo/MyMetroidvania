using MyMetroidVania.Data.ScriptableObjects;
using System;
using System.Collections;
using UnityEngine;

namespace MyMetroidVania.Entity
{
    /// <summary>
    /// ステータスを管理
    /// </summary>
    public class StatusManager : MonoBehaviour, IDamageDealer
    {
        [SerializeField, Tooltip("ステータスの初期値")]
        private Status _defaultStatus = null;
        public Status DefaultStatus => _defaultStatus;
        // 現在のステータス
        private Status _currentStatus = null;
        public Status CurrentStatus => _currentStatus;
        private bool _isInvincible = false;
        /// <summary>
        /// 死亡しているかどうか
        /// </summary>
        public bool IsDead => _currentStatus.Life <= 0;

        /// <summary>
        /// 被ダメージ時に発火するイベント
        /// </summary>
        public event Action OnDamageTaken;
        /// <summary>
        /// 死亡時に発火するイベント
        /// </summary>
        public event Action OnDead;
        /// <summary>
        /// 最大ライフが変更された場合に発火するイベント
        /// </summary>
        public event Action<int> OnLifeCountChanged;
        /// <summary>
        /// ライフが変更された場合に発火するイベント
        /// </summary>
        public event Action<int> OnLifeChanged;

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Start()
        {
            InitializeStatus();
        }

        /// <summary>
        /// ライフの回復処理
        /// </summary>
        public void Heal()
        {
            int healValue = _defaultStatus.Life - _currentStatus.Life;
            _currentStatus.AddLife(healValue);
            OnLifeChanged?.Invoke(_currentStatus.Life);
        }

        /// <summary>
        /// ダメージを受ける処理
        /// </summary>
        /// <param name="damage">受けるダメージ量</param>
        public void TakeDamage(int damage)
        {
            // 無敵状態、または体力が0以下の場合は処理をスキップ
            if (_isInvincible || (_currentStatus.Life <= 0)) return;

            _currentStatus.AddLife(-damage); //ダメージなので負の数で計算
            if (_currentStatus.Life <= 0)
            {
                OnDead?.Invoke();
            }
            else
            {
                OnDamageTaken?.Invoke();
                OnLifeChanged?.Invoke(_currentStatus.Life);
                StartCoroutine(OnTakenDamage());
            }
        }

        /// <summary>
        /// ダメージ発生後の無敵化
        /// </summary>
        private IEnumerator OnTakenDamage()
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

            // ステータスのUIの更新
            OnLifeCountChanged?.Invoke(_defaultStatus.Life);
            OnLifeChanged?.Invoke(_currentStatus.Life);
        }

        /// <summary>
        /// ライフ最大値を上昇させる
        /// </summary>
        public void LifeUp()
        {
            _defaultStatus.AddLife(1);
            // 全回復
            OnLifeCountChanged?.Invoke(_defaultStatus.Life);
            Heal();
        }

        /// <summary>
        /// ライフ最大値を更新（初期化時）
        /// </summary>
        public void UpdateLife(int value)
        {
            _defaultStatus.SetLife(value);
            _currentStatus.SetLife(value);
            OnLifeCountChanged?.Invoke(_defaultStatus.Life);
        }

        /// <summary>
        /// 攻撃力を取得する
        /// </summary>
        /// <returns>攻撃力の値</returns>
        public int GetAttackPower()
        {
            return _currentStatus.AttackPower;
        }
    }
}
