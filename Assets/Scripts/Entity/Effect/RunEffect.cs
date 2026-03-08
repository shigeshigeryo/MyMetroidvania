using UnityEngine;

namespace MyMetroidVania.Entity.Effect
{
    public class RunEffect : MonoBehaviour
    {
        private Animator _animator;
        private readonly int _playToRightId = Animator.StringToHash("PlayToRight");
        private readonly int _playToLeftId = Animator.StringToHash("PlayToLeft");

        /// <summary>
        /// アニメーションを再生する
        /// </summary>
        /// <param name="isRight">右向きに走っているかどうか</param>
        public void PlayAnimation(bool isRight)
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }

            // 右向きか左向きかでアニメーションを変える
            _animator.SetTrigger(isRight ? _playToRightId : _playToLeftId);
        }
    }
}