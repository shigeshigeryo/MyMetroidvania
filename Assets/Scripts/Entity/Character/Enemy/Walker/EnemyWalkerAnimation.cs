using UnityEngine;

namespace MyMetroidVania.Entity.Character.Enemy.Walker
{
    public class EnemyWalkerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private EnemyWalker _enemyWalker;
        [SerializeField] private StatusManager _statusManager;

        private static readonly int _walkSpeedId = Animator.StringToHash("WalkSpeed"); // 移動速度
        private static readonly int _moveId = Animator.StringToHash("IsMove"); // 移動しているか
        private static readonly int _takenDamageId = Animator.StringToHash("TakenDamage"); // 被弾アニメーション
        private static readonly int _deadId = Animator.StringToHash("Dead"); // 死亡アニメーション


        /// <summary>
        /// アニメーションイベントの購読
        /// </summary>
        private void Start()
        {
            _enemyWalker.OnWalked += StartMove;
            _enemyWalker.OnChased += StartMove;
            _enemyWalker.OnStopped += StopMove;

            _statusManager.OnDamageTaken += TriggerTakenDamage;
            _statusManager.OnDead += TriggerDead;
        }

        private void FixedUpdate()
        {
            // 移動速度を常に更新
            _animator.SetFloat(_walkSpeedId, _enemyWalker.CurrentSpeed);
        }

        /// <summary>
        /// 移動フラグを立てる
        /// </summary>
        private void StartMove()
        {
            _animator.SetBool(_moveId, true);
        }
        /// <summary>
        /// 移動フラグを下げる
        /// </summary>
        private void StopMove()
        {
            _animator.SetBool(_moveId, false);
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

        private void OnDestroy()
        {
            _enemyWalker.OnWalked -= StartMove;
            _enemyWalker.OnChased -= StartMove;
            _enemyWalker.OnStopped -= StopMove;

            _statusManager.OnDamageTaken -= TriggerTakenDamage;
            _statusManager.OnDead -= TriggerDead;
        }
    }
}