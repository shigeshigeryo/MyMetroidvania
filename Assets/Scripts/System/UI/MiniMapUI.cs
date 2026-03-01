using UnityEngine;

namespace MyMetroidVania.System.UI
{
    public class MiniMapUI : MonoBehaviour
    {
        private void Start()
        {
            GameManager.Instance.OnToggledMiniMap += ToggleMiniMap;
            foreach(Transform t in transform)
            {
                t.gameObject.SetActive(false);
            }
        }

        private void ToggleMiniMap()
        {
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(!t.gameObject.activeSelf);
            }
        }
    }
}