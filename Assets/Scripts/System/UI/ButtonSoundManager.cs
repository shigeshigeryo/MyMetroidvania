using MyMetroidVania.Data.ScriptableObjects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyMetroidVania.System.UI
{
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

        public void OnDeselect(BaseEventData eventData)
        {
            AudioManager.Instance.PlayOneShotSe(_deselectSound);
        }

        private void OnDestroy()
        {
            if (_button != null)
            {
                _button.onClick.RemoveAllListeners();
            }
        }
    }
}