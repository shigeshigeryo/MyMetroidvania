using MyMetroidVania.Data.ScriptableObjects;
using System;
using System.Collections;
using UnityEngine;

namespace MyMetroidVania.Entity
{
    public class StatusManager : MonoBehaviour, IDamageDealer
    {
        [SerializeField, Tooltip("ステータスの初期値")]
        private Status _defaultStatus = null;
        // 現在のステータス
        private Status _currentStatus = null;
        public Status CurrentStatus => _currentStatus;
        private bool _isInvincible = false;
        public bool IsDead => _currentStatus.Life <= 0;

        public event Action OnDamageTaken;
        public event Action OnDead;
        public event Action<int> OnLifeCountChanged; // ライフ数のUIを更新
        public event Action<int> OnLifeChanged; // ライフのUIを更新

        private void Start()
        {
            InitializeStatus();
        }

        public void Heal()
        {
            int healValue = _defaultStatus.Life - _currentStatus.Life;
            _currentStatus.UpdateLife(healValue);
            OnLifeChanged?.Invoke(_currentStatus.Life);
        }

        public void TakeDamage(int damage)
        {
            // 無敵状態、または体力が0以下の場合は処理をスキップ
            if (_isInvincible || (_currentStatus.Life <= 0)) return;

            _currentStatus.UpdateLife(-damage); //ダメージなので負の数で計算
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
            _defaultStatus.UpdateLife(1);
            // 全回復
            _currentStatus.UpdateLife(_defaultStatus.Life - _currentStatus.Life);

            OnLifeCountChanged?.Invoke(_defaultStatus.Life);
            OnLifeChanged?.Invoke(_currentStatus.Life);
        }

        public int GetAttackPower()
        {
            return _currentStatus.AttackPower;
        }
    }
}