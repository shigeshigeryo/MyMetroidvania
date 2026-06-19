using UnityEngine;

namespace MyMetroidVania.System.UI
{
    /// <summary>
    /// ゲームクリアのUIを管理
    /// </summary>
    public class GameClearUI : MonoBehaviour
    {
        [SerializeField] private Animator _animator = null;
        private static readonly int _showId = Animator.StringToHash("Show");

        /// <summary>
        /// 表示する
        /// </summary>
        public void Show()
        {
            _animator.SetTrigger(_showId);
        }

        /// <summary>
        /// Showアニメーション完了時に発火
        /// </summary>
        public void FinishAnimation()
        {
            GameManager.Instance.LoadTitleScene();
        }
    }
}
