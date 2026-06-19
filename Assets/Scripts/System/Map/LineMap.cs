using UnityEngine;

namespace MyMetroidVania.System.Map
{
    /// <summary>
    /// エリアとエリアをつなぐ線の管理
    /// </summary>
    public class LineMap : MonoBehaviour
    {
        /// <summary>
        /// 初期化処理
        /// 最初は非表示
        /// </summary>
        private void Awake()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 表示する
        /// </summary>
        public void SetActive()
        {
            if (!gameObject.activeSelf) gameObject.SetActive(true);
        }
    }
}
