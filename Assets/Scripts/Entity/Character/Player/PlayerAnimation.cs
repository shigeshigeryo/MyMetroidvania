using UnityEngine;

namespace MyMetroidVania.Entity.Character.Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private StatusManager _statusManager;

        private static readonly int _runId = Animator.StringToHash("IsRun");
        private static readonly int _jumpId = Animator.StringToHash("Jump");
        private static readonly int _groundedId = Animator.StringToHash("IsGrounded");
        private static readonly int _fallId = Animator.StringToHash("IsFall");
        private static readonly int _takenDamageId = Animator.StringToHash("TakenDamage");


        /// <summary>
        /// アニメーションイベントの購読
        /// </summary>
        private void Start()
        {
            _statusManager.OnDamageTaken += TriggerTakenDamage;
        }

        public void UpdateParam(Vector2 vel, bool isGrounded)
        {
            _animator.SetBool(_runId, Mathf.Abs(vel.x) > 0.01f);
            _animator.SetBool(_fallId, vel.y < -0.01f);
            _animator.SetBool(_groundedId, isGrounded);
        }

        /// <summary>
        /// ジャンプモーションをスタート
        /// </summary>
        public void TriggerJump()
        {
            _animator.SetTrigger(_jumpId);
        }

        /// <summary>
        /// 被弾モーションをスタート
        /// </summary>
        public void TriggerTakenDamage()
        {
            _animator.SetTrigger(_takenDamageId);
        }

        private void OnDestroy()
        {
            _statusManager.OnDamageTaken -= TriggerTakenDamage;
        }
    }
}