using PlayerInputActions = MyMetroidVania.Entity.Character.Player.PlayerInputActions;
using System;
using UnityEngine;
using MyMetroidVania.System.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace MyMetroidVania.System
{
    /// <summary>
    /// ゲーム全体を管理
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance = null;
        /// <summary>
        /// シングルトンインスタンス
        /// </summary>
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

        /// <summary>
        /// ミニマップ切り替え時に発火するイベント
        /// </summary>
        public event Action OnToggledMiniMap;
        [SerializeField, Tooltip("画面遷移フェードUI")] private TransitionUI _transitionUI = null;
        [SerializeField, Tooltip("クリアUI")] private GameClearUI _clearUI = null;

        /// <summary>
        /// ゲームのステート
        /// </summary>
        public enum GameState
        {
            Transition, // 遷移中
            Play, // プレイ
            Pause, // ポーズ
            Clear // クリア
        }
        private GameState _currentState = GameState.Transition;
        /// <summary>
        /// プレイ中かどうか
        /// </summary>
        public bool IsPlay => _currentState == GameState.Play;

        /// <summary>
        /// シングルトン化処理
        /// </summary>
        private void Awake()
        {
            // シングルトン
            if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
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
        /// デバッグコマンド用処理
        /// </summary>
        private void Update()
        {
            // デバッグコマンド タイトルシーンに遷移 c, tキー同時押し
            if (Keyboard.current.cKey.isPressed && Keyboard.current.tKey.wasPressedThisFrame)
            {
                SceneManager.LoadScene("Title");
            }
        }

        /// <summary>
        /// エリア移動時処理
        /// </summary>
        public void ChangeArea(string areaId, Vector3 spawnPosition)
        {
            ShowTransition();
            WorldManager.Instance.ChangeArea(areaId, spawnPosition);
        }

        /// <summary>
        /// 遷移状態を表示
        /// </summary>
        public void ShowTransition()
        {
            _currentState = GameState.Transition;
            _transitionUI.Show();
        }

        /// <summary>
        /// Play状態に遷移
        /// </summary>
        public void ChangeStatePlay()
        {
            _currentState = GameState.Play;
        }

        /// <summary>
        /// クリアする
        /// </summary>
        public void GameClear()
        {
            _currentState = GameState.Clear;
            AudioManager.Instance.FadeOutBGM();
            _clearUI.Show();
        }

        /// <summary>
        /// タイトルに遷移する
        /// </summary>
        public void LoadTitleScene()
        {
            SceneManager.LoadScene("Title");
        }

        /// <summary>
        /// イベント購読解除処理
        /// </summary>
        private void OnDestroy()
        {
            // 購読解除
            InputActions.UI.ToggleMiniMap.started -= _ => OnToggledMiniMap?.Invoke();
            InputActions.UI.ToggleMiniMap.canceled -= _ => OnToggledMiniMap?.Invoke();
        }
    }
}
