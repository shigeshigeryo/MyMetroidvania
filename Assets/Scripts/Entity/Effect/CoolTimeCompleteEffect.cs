using UnityEngine;

namespace MyMetroidVania.Entity.Effect
{
    /// <summary>
    /// クールタイム完了エフェクト
    /// </summary>
    public class CoolTimeCompleteEffect : MonoBehaviour
    {
        private Animator _animator;
        private readonly int _playCompleteId = Animator.StringToHash("Complete");

        /// <summary>
        /// アニメーションを再生する
        /// </summary>
        public void PlayAnimation()
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }

            _animator.SetTrigger(_playCompleteId);
        }
    }
}
