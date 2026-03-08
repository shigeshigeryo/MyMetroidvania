using MyMetroidVania.Utility;
using UnityEngine;

namespace MyMetroidVania.System.UI
{
    public class TutorialUI : MonoBehaviour
    {
        [SerializeField, Tooltip("ボックスキャスト（この範囲にプレイヤーがいる場合にチュートリアルが表示される）")]
        private BoxCaster _playerChecker = null;
        [SerializeField] Animator _animator;

        private static int _isPlayerCastedID = Animator.StringToHash("IsPlayerCasted");

        private void Update()
        {
            // プレイヤーが近づくとチュートリアルを表示
            _animator.SetBool(_isPlayerCastedID, _playerChecker.IsCasted);
        }
    }
}