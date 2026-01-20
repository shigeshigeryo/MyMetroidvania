using MyMetroidVania.Entity.Character.Player;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyMetroidVania.System
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; } = null;

        private PlayerInputActions _playerInputActions;
        public PlayerInputActions PlayerInputActions => _playerInputActions;
        
        public event Action OnToggledMiniMap;

        private void Awake()
        {
            // シングルトン
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            _playerInputActions = new PlayerInputActions();
        }

        private void Start()
        {
            // UIの操作を登録
            _playerInputActions.UI.Enable();
            _playerInputActions.UI.ToggleMiniMap.started += OnToggleMiniMap;
            _playerInputActions.UI.ToggleMiniMap.canceled += OnToggleMiniMap;
        }

        /// <summary>
        /// ミニマップの切り替えを行う
        /// 実態はMiniMapUI参照
        /// </summary>
        private void OnToggleMiniMap(InputAction.CallbackContext _)
        {
            OnToggledMiniMap?.Invoke();
        }

        private void OnDestroy()
        {
            // 購読解除
            _playerInputActions.UI.ToggleMiniMap.started -= OnToggleMiniMap;
            _playerInputActions.UI.ToggleMiniMap.canceled -= OnToggleMiniMap;
        }
    }
}