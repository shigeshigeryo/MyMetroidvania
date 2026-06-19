using UnityEngine;

namespace MyMetroidVania.System.UI
{
    /// <summary>
    /// ミニマップUIの管理
    /// </summary>
    public class MiniMapUI : MonoBehaviour
    {
        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Start()
        {
            GameManager.Instance.OnToggledMiniMap += ToggleMiniMap;
            foreach(Transform t in transform)
            {
                t.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// ミニマップ切り替え処理
        /// </summary>
        private void ToggleMiniMap()
        {
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(!t.gameObject.activeSelf);
            }
        }
    }
}
