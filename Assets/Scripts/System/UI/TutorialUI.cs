using MyMetroidVania.Utility;
using UnityEngine;

namespace MyMetroidVania.System.UI
{
    /// <summary>
    /// チュートリアルのUIを管理
    /// </summary>
    public class TutorialUI : MonoBehaviour
    {
        [SerializeField, Tooltip("ボックスキャスト（この範囲にプレイヤーがいる場合にチュートリアルが表示される）")]
        private BoxCaster _playerChecker = null;
        [SerializeField] Animator _animator;

        private static int _isPlayerCastedID = Animator.StringToHash("IsPlayerCasted");

        /// <summary>
        /// プレイヤーが近づくとチュートリアルを表示
        /// </summary>
        private void Update()
        {
            _animator.SetBool(_isPlayerCastedID, _playerChecker.IsCasted);
        }
    }
}
