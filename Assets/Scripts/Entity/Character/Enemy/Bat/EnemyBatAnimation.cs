using System;
using UnityEngine;

namespace MyMetroidVania.Entity.Character.Enemy.Bat
{
    /// <summary>
    /// コウモリのアニメーションを管理
    /// </summary>
    public class EnemyBatAnimation : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private EnemyBat _enemyBat;
        [SerializeField] private StatusManager _statusManager;
        [SerializeField] protected SpriteRenderer _renderer = null;

        private static readonly int _moveSpeedId = Animator.StringToHash("MoveSpeed"); // 移動速度
        private static readonly int _attackId = Animator.StringToHash("Attack"); // 被弾アニメーション
        private static readonly int _takenDamageId = Animator.StringToHash("TakenDamage"); // 被弾アニメーション
        private static readonly int _deadId = Animator.StringToHash("Dead"); // 死亡アニメーション

        /// <summary>
        /// 攻撃時に発火するイベント
        /// </summary>
        public event Action OnAttack;
        /// <summary>
        /// 死亡アニメーション完了時に発火するイベント
        /// </summary>
        public event Action OnCompleteDeadAnimation;

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
        public void UpdateParam(Vector2 velocity)
        {
            // 移動速度を常に更新
            _animator.SetFloat(_moveSpeedId, Mathf.Abs(velocity.sqrMagnitude));

            SetFlip(velocity.x);
        }

        /// <summary>
        /// 入力方向によってスプライトの向きを変える
        /// </summary>
        /// <param name="velocityX">X軸の速度</param>
        private void SetFlip(float velocityX)
        {
            // 入力がない場合はFlipの更新を行わない
            if (velocityX > 0.01f)
            {
                _renderer.flipX = false;
            }
            else if (velocityX < -0.01f)
            {
                _renderer.flipX = true;
            }
        }

        /// <summary>
        /// 攻撃アニメーションをスタート
        /// </summary>
        public void TriggerAttack()
        {
            _animator.SetTrigger(_attackId);
        }

        /// <summary>
        /// 体当たり攻撃の開始
        /// </summary>
        public void StartAttack()
        {
            OnAttack?.Invoke();
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
            OnCompleteDeadAnimation?.Invoke();
        }

        /// <summary>
        /// イベント購読解除処理
        /// </summary>
        private void OnDestroy()
        {
            _statusManager.OnDamageTaken -= TriggerTakenDamage;
            _statusManager.OnDead -= TriggerDead;
        }
    }
}