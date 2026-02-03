using UnityEngine;

namespace MyMetroidVania.System.UI
{
    public class TransitionUI : MonoBehaviour
    {
        [SerializeField] Animator _animator = null;
        private readonly int _showId = Animator.StringToHash("Show");

        public void Show()
        {
            if (_animator != null)
            {
                _animator.SetTrigger(_showId);
            }
        }
    }
}