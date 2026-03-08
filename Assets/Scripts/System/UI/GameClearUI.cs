using UnityEngine;

namespace MyMetroidVania.System.UI
{
    public class GameClearUI : MonoBehaviour
    {
        [SerializeField] private Animator _animator = null;
        private static readonly int _showId = Animator.StringToHash("Show");

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