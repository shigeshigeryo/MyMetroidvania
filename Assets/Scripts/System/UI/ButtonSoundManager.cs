using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyMetroidVania.System.UI
{
    /// <summary>
    /// サウンドがなるボタンを管理
    /// </summary>
    public class ButtonSoundManager : MonoBehaviour, IDeselectHandler
    {
        private Button _button = null;
        private SoundData _deselectSound = null;

        private void Start()
        {
            _button = GetComponent<Button>();
            _deselectSound = AudioManager.Instance.GetSe("SE_Deselect");

            if (_button != null)
            {
                SoundData submitButton = AudioManager.Instance.GetSe("SE_Submit");
                _button.onClick.AddListener(() => AudioManager.Instance.PlayOneShotSe(submitButton));  
            }
        }

        /// <summary>
        /// deselect時にサウンドを鳴らす
        /// </summary>
        /// <param name="eventData">鳴らすサウンド</param>
        public void OnDeselect(BaseEventData eventData)
        {
            AudioManager.Instance.PlayOneShotSe(_deselectSound);
        }

        /// <summary>
        /// clean処理
        /// </summary>
        private void OnDestroy()
        {
            if (_button != null)
            {
                _button.onClick.RemoveAllListeners();
            }
        }
    }
}
