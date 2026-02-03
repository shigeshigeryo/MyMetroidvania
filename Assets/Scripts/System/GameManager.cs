using PlayerInputActions = MyMetroidVania.Entity.Character.Player.PlayerInputActions;
using System;
using UnityEngine;
using MyMetroidVania.System.UI;

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
        [SerializeField, Tooltip("画面遷移フェードUI")] private TransitionUI _transitionUI = null;


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
            InputActions.UI.ToggleMiniMap.started += _ =>  OnToggledMiniMap?.Invoke();
            InputActions.UI.ToggleMiniMap.canceled += _ => OnToggledMiniMap?.Invoke();
        }

        /// <summary>
        /// エリア移動時処理
        /// </summary>
        public void ChangeArea(string areaId, Vector3 spawnPosition)
        {
            _transitionUI.Show();
            WorldManager.Instance.ChangeArea(areaId, spawnPosition);
        }


        private void OnDestroy()
        {
            // 購読解除
            InputActions.UI.ToggleMiniMap.started -= _ => OnToggledMiniMap?.Invoke();
            InputActions.UI.ToggleMiniMap.canceled -= _ => OnToggledMiniMap?.Invoke();
        }
    }
}