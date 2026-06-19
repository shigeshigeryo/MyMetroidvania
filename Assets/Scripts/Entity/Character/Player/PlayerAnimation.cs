using UnityEngine;

namespace MyMetroidVania.Entity.Character.Player
{
    /// <summary>
    /// プレイヤーのアニメーションを管理
    /// </summary>
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private StatusManager _statusManager;

        private static readonly int _runId = Animator.StringToHash("IsRun");
        private static readonly int _jumpId = Animator.StringToHash("Jump");
        private static readonly int _groundedId = Animator.StringToHash("IsGrounded");
        private static readonly int _VelocityYId = Animator.StringToHash("VelocityY");
        private static readonly int _takenDamageId = Animator.StringToHash("TakenDamage");


        /// <summary>
        /// アニメーションイベントの購読
        /// </summary>
        private void Start()
        {
            _statusManager.OnDamageTaken += TriggerTakenDamage;
        }

        /// <summary>
        /// 移動速度や接地状態を更新
        /// </summary>
        /// <param name="vel">移動速度</param>
        /// <param name="isGrounded">接地状態</param>
        public void UpdateParam(Vector2 vel, bool isGrounded)
        {
            _animator.SetBool(_runId, Mathf.Abs(vel.x) > 0.01f);
            _animator.SetBool(_groundedId, isGrounded);
            _animator.SetFloat(_VelocityYId, vel.y);
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

        /// <summary>
        /// イベント購読解除処理
        /// </summary>
        private void OnDestroy()
        {
            _statusManager.OnDamageTaken -= TriggerTakenDamage;
        }
    }
}
