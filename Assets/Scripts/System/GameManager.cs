using PlayerInputActions = MyMetroidVania.Entity.Character.Player.PlayerInputActions;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyMetroidVania.System
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance = null;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    // GameManagerはシーンに必ず用意する
                    _instance = FindFirstObjectByType<GameManager>();
                }
                return _instance;
            }
        }

        private PlayerInputActions _playerInputActions = null;
        public PlayerInputActions InputActions
        {
            get
            {
                if (_playerInputActions == null)
                {
                    // 初回呼び出しの場合はインスタンスを生成する
                    _playerInputActions = new PlayerInputActions();
                }
                return _playerInputActions;
            }
        }

        public event Action OnToggledMiniMap;

        private void Awake()
        {
            // シングルトン
            if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            // UIの操作を登録
            InputActions.UI.Enable();
            InputActions.UI.ToggleMiniMap.started += OnToggleMiniMap;
            InputActions.UI.ToggleMiniMap.canceled += OnToggleMiniMap;
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
            InputActions.UI.ToggleMiniMap.started -= OnToggleMiniMap;
            InputActions.UI.ToggleMiniMap.canceled -= OnToggleMiniMap;
        }
    }
}