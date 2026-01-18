using UnityEngine;

namespace MyMetroidVania.Entity.Character.Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Player _player;
        [SerializeField] private StatusManager _statusManager;

        private static readonly int _runId = Animator.StringToHash("IsRun");
        private static readonly int _jumpId = Animator.StringToHash("Jump");
        private static readonly int _fallId = Animator.StringToHash("IsFall");
        private static readonly int _takenDamageId = Animator.StringToHash("TakenDamage");


        /// <summary>
        /// アニメーションイベントの購読
        /// </summary>
        private void Start()
        {
            _player.OnIdle += StopRun;
            _player.OnIdle += StopFall;
            _player.OnRun += StartRun;
            _player.OnRun += StopFall;
            _player.OnJumped += TriggerJump;
            _player.OnJumped += StopRun;
            _player.OnFallen += StartFall;
            _player.OnFallen += StopRun;

            _statusManager.OnDamageTaken += TriggerTakenDamage;
        }

        /// <summary>
        /// 走るモーションを再生
        /// </summary>
        private void StartRun()
        {
            _animator.SetBool(_runId, true);
        }

        /// <summary>
        /// 走るモーションをストップ
        /// </summary>
        private void StopRun()
        {
            _animator.SetBool(_runId, false);
        }

        /// <summary>
        /// 落下モーションをスタート
        /// </summary>
        private void StartFall()
        {
            _animator.SetBool(_fallId, true);
        }

        /// <summary>
        /// 落下モーションをストップ
        /// </summary>
        private void StopFall()
        {
            _animator.SetBool(_fallId, false);
        }

        /// <summary>
        /// ジャンプモーションをスタート
        /// </summary>
        private void TriggerJump()
        {
            _animator.SetTrigger(_jumpId);
        }

        /// <summary>
        /// 被弾モーションをスタート
        /// </summary>
        private void TriggerTakenDamage()
        {
            _animator.SetTrigger(_takenDamageId);
        }

        private void OnDestroy()
        {
            _player.OnIdle -= StopRun;
            _player.OnIdle -= StopFall;
            _player.OnRun -= StartRun;
            _player.OnRun -= StopFall;
            _player.OnJumped -= TriggerJump;
            _player.OnJumped -= StopRun;
            _player.OnFallen -= StartFall;
            _player.OnFallen -= StopRun;

            _statusManager.OnDamageTaken -= TriggerTakenDamage;
        }
    }
}