using MyMetroidVania.Entity.Character.Player;
using UnityEngine;
using UnityEngine.UI;

namespace MyMetroidVania.System.UI
{
    public class HookUI : MonoBehaviour
    {
        [SerializeField] private Player _player;
        [SerializeField, Tooltip("フックUIイメージ")] private Image _hookUiImage;

        private void Start()
        {
            _player.OnCoolHook += UpdateValue;
            _hookUiImage.fillAmount = 0;
        }

        /// <summary>
        /// フックのCTの進捗を反映
        /// </summary>
        /// <param name="value"></param>
        private void UpdateValue(float value)
        {
            _hookUiImage.fillAmount = Mathf.Clamp01(value);
        }

        private void OnDestroy()
        {
            _player.OnCoolHook -= UpdateValue;
        }
    }
}