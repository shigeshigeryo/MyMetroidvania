using UnityEngine;

namespace MyMetroidVania.System.UI
{
    /// <summary>
    /// エリア遷移時のUIを管理
    /// </summary>
    public class TransitionUI : MonoBehaviour
    {
        [SerializeField] Animator _animator = null;
        private readonly int _showId = Animator.StringToHash("Show");

        /// <summary>
        /// 表示する
        /// </summary>
        public void Show()
        {
            if (_animator != null)
            {
                _animator.SetTrigger(_showId);
            }
        }

        /// <summary>
        /// トランジションUIフェード開始時に発火
        /// </summary>
        public void StartFade()
        {
            GameManager.Instance.ChangeStatePlay();
        }
    }
}
