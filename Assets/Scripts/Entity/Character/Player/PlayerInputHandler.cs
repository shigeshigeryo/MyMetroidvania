using MyMetroidVania.System;
using System;
using UnityEngine;

namespace MyMetroidVania.Entity.Character.Player
{
    /// <summary>
    /// プレイヤーの入力周りの管理
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

        // 移動入力方向
        private Vector2 _inputDirection = Vector2.zero;
        public Vector2 InputDirection => _inputDirection;

        // ジャンプボタンが押されているかどうか
        public bool IsPressedJumpButton => Actions.Player.Jump.IsPressed();

        // ボタンイベント（別クラスからサブスクする）
        public event Action OnJumpStarted;
        public event Action OnHookStarted;
        public event Action OnHookCanceled;
        public event Action OnAttackStarted;
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

        private void Update()
        {
            _inputDirection = Actions.Player.Move.ReadValue<Vector2>();
        }
    }
}