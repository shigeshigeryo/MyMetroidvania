using PlayerInputActions = MyMetroidVania.Entity.Character.Player.PlayerInputActions;
using System;
using UnityEngine;
using MyMetroidVania.System.UI;
using UnityEngine.SceneManagement;

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
        [SerializeField, Tooltip("クリアUI")] private GameClearUI _clearUI = null;

        public enum GameState
        {
            Transition, // 遷移中
            Play, // プレイ
            Pause, // ポーズ
            Clear // クリア
        }
        private GameState _currentState = GameState.Transition;
        public bool IsPlay => _currentState == GameState.Play;

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
            AudioManager.Instance.GetAndPlayBgm("BGM_InGame");

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

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
            _currentState = GameState.Transition;
            _transitionUI.Show();
            WorldManager.Instance.ChangeArea(areaId, spawnPosition);
        }

        public void ChangeStatePlay()
        {
            _currentState = GameState.Play;
        }

        public void GameClear()
        {
            _currentState = GameState.Clear;
            AudioManager.Instance.FadeOutBGM();
            _clearUI.Show();
        }

        public void LoadTitleScene()
        {
            SceneManager.LoadScene("Title");
        }

        private void OnDestroy()
        {
            // 購読解除
            InputActions.UI.ToggleMiniMap.started -= _ => OnToggledMiniMap?.Invoke();
            InputActions.UI.ToggleMiniMap.canceled -= _ => OnToggledMiniMap?.Invoke();
        }
    }
}