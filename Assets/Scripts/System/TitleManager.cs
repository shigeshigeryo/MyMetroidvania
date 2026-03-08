using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MyMetroidVania.System
{
    public class TitleManager : MonoBehaviour
    {
        public static TitleManager Instance { get; private set; } = null;

        [SerializeField] private Animator _animator = null;
        [SerializeField] private Button _startButton = null;
        [SerializeField] private Button _exitButton = null;

        private static readonly int _outroId = Animator.StringToHash("Outro");

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            AudioManager.Instance.GetAndPlayBgm("BGM_Title");

            if (_startButton != null)
            {
                _startButton.onClick.AddListener(() => _animator.SetTrigger(_outroId));
                _startButton.onClick.AddListener(() => AudioManager.Instance.FadeOutBGM());
                _startButton.Select();
            }

            if (_exitButton != null)
            {
                _exitButton.onClick.AddListener(QuitGame);
            }
        }

        private void Update()
        {
            Gamepad pad = Gamepad.current;
            if (pad == null) return;

            // ボタン選択が外れてしまった際の対応
            if (pad.aButton.wasPressedThisFrame && EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(_startButton.gameObject);
            }
        }

        public void LoadInGameScene()
        {
            SceneManager.LoadScene("InGameScene");
        }

        private void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }

        private void OnDestroy()
        {
            _startButton.onClick.RemoveAllListeners();
            _exitButton.onClick.RemoveAllListeners();
        }
    }
}