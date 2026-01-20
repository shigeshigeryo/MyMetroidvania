using UnityEngine;
using UnityEngine.UI;

namespace MyMetroidVania.System.UI
{
    public class MiniMapUI : MonoBehaviour
    {
        [SerializeField, Tooltip("マップイメージ")] private RawImage _mapImg;

        private void Start()
        {
            GameManager.Instance.OnToggledMiniMap += ToggleMiniMap;
            _mapImg.enabled = false;
        }

        private void ToggleMiniMap()
        {
            _mapImg.enabled = !_mapImg.enabled;
        }
    }
}