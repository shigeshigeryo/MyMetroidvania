using MyMetroidVania.Entity.Character.Enemy.Walker;
using UnityEngine;

namespace MyMetroidVania.Entity.Character.Enemy.Bat
{
    public class EnemyBatAnimation : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private EnemyBat _enemyBat;
        [SerializeField] private StatusManager _statusManager;

        private static readonly int _moveSpeedId = Animator.StringToHash("MoveSpeed"); // 移動速度
        private static readonly int _takenDamageId = Animator.StringToHash("TakenDamage"); // 被弾アニメーション
        private static readonly int _deadId = Animator.StringToHash("Dead"); // 死亡アニメーション


        /// <summary>
        /// アニメーションイベントの購読
        /// </summary>
        private void Start()
        {
            _statusManager.OnDamageTaken += TriggerTakenDamage;
            _statusManager.OnDead += TriggerDead;
        }

        /// <summary>
        /// アニメーターで使用しているパラメータを更新
        /// </summary>
        /// <param name="velocity">移動速度</param>
        public void UpdateParam(float velocity)
        {
            // 移動速度を常に更新
            _animator.SetFloat(_moveSpeedId, velocity);
        }

        /// <summary>
        /// 被弾アニメーションをスタート
        /// </summary>
        private void TriggerTakenDamage()
        {
            _animator.SetTrigger(_takenDamageId);
        }

        /// <summary>
        /// 死亡アニメーションをスタート
        /// </summary>
        private void TriggerDead()
        {
            _animator.SetTrigger(_deadId);
        }

        /// <summary>
        /// Deadアニメーションの最後で発火
        /// </summary>
        public void Dead()
        {
            _enemyBat.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _statusManager.OnDamageTaken -= TriggerTakenDamage;
            _statusManager.OnDead -= TriggerDead;
        }
    }
}