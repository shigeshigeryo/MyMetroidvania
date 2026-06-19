using MyMetroidVania.System;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyMetroidVania.Entity.Character.Player
{
    /// <summary>
    /// プレイヤーの入の管理
    /// </summary>
    public class PlayerInputHandler : MonoBehaviour
    {
        private PlayerInputActions _actions = null;
        private PlayerInputActions Actions
        {
            get
            {
                if (_actions == null)
                {
                    // 初回呼び出しの場合はGameManagerから取得してくる。
                    return _actions = GameManager.Instance.InputActions;
                }
                else
                {
                    return _actions;
                }
            }
        }

        [Header("振動")]
        [SerializeField, Tooltip("振動のパワー"), Range(0f, 1f)] private float _vibrationPower = 0.4f;
        [SerializeField, Tooltip("振動の持続秒数")] private float _vibrationSec = 0.1f;

        /// <summary>
        /// 移動入力方向
        /// </summary>
        public Vector2 InputDirection { get; private set; } = Vector2.zero;
        /// <summary>
        /// 最後に入力した方向
        /// </summary>
        public Vector2 LastInputDirection { get; private set; } = Vector2.zero;

        /// <summary>
        /// ジャンプボタンが押されている場合true
        /// </summary>
        public bool IsPressedJumpButton => Actions.Player.Jump.IsPressed();

        /// <summary>
        /// ジャンプ時に発火するイベント
        /// </summary>
        public event Action OnJumpStarted;
        /// <summary>
        /// フックを開始した時に発火するイベント
        /// </summary>
        public event Action OnHookStarted;
        /// <summary>
        /// フックをキャンセルした時に発火するイベント
        /// </summary>
        public event Action OnHookCanceled;
        /// <summary>
        /// 攻撃時に発火するイベント
        /// </summary>
        public event Action OnAttackStarted;
        /// <summary>
        /// インタラクト時に発火するイベント
        /// </summary>
        public event Action OnInteractStarted;

        /// <summary>
        /// InputSystemのサブスク
        /// </summary>
        private void OnEnable()
        {
            Actions.Player.Enable();
            Actions.Player.Jump.started += _ => OnJumpStarted?.Invoke();
            Actions.Player.Hook.performed += _ => OnHookStarted?.Invoke();
            Actions.Player.Hook.canceled += _ => OnHookCanceled?.Invoke();
            Actions.Player.Attack.started += _ => OnAttackStarted?.Invoke();
            Actions.Player.Interact.started += _ => OnInteractStarted?.Invoke();
        }

        /// <summary>
        /// InputSystemのサブスク解除
        /// </summary>
        private void OnDisable()
        {
            Actions.Player.Disable();
            Actions.Player.Jump.started -= _ => OnJumpStarted?.Invoke();
            Actions.Player.Hook.performed -= _ => OnHookStarted?.Invoke();
            Actions.Player.Hook.canceled -= _ => OnHookCanceled?.Invoke();
            Actions.Player.Attack.started -= _ => OnAttackStarted?.Invoke();
            Actions.Player.Interact.started -= _ => OnInteractStarted?.Invoke();
        }

        /// <summary>
        /// 入力情報の取得
        /// </summary>
        private void Update()
        {
            if (GameManager.Instance.IsPlay)
            {
                var dir = Actions.Player.Move.ReadValue<Vector2>();
                // 入力情報がない場合は値の保持をしない
                if (dir.sqrMagnitude > 0.01f)
                {
                    LastInputDirection = dir;
                }
                InputDirection = dir;
            }
            else
            {
                // 遷移中の移動を防ぐ
                InputDirection = Vector2.zero;
            }
        }

        /// <summary>
        /// コントローラーを振動させる
        /// </summary>
        public void VibrateController()
        {
            StartCoroutine(OnVibrateController());
        }

        /// <summary>
        /// コントローラーの振動処理の管理
        /// </summary>
        /// <returns></returns>
        private IEnumerator OnVibrateController()
        {
            var pad = Gamepad.current;
            if (pad == null) yield break;

            // 振動開始
            pad.SetMotorSpeeds(_vibrationPower, _vibrationPower);
            yield return new WaitForSeconds(_vibrationSec);

            // 振動終了
            pad.SetMotorSpeeds(0f, 0f);
        }
    }
}
