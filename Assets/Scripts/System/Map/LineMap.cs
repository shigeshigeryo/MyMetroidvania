using UnityEngine;

namespace MyMetroidVania.System.Map
{
    public class LineMap : MonoBehaviour
    {
        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void SetActive()
        {
            if (!gameObject.activeSelf) gameObject.SetActive(true);
        }
    }
}