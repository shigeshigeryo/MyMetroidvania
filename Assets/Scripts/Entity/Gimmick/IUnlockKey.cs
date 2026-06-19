using System;

namespace MyMetroidVania.Entity.Gimmick
{
    /// <summary>
    /// ギミックの解除のキーとなるギミックに実装するインタフェース
    /// </summary>
    public interface IUnlockKey
    {
        /// <summary>
        /// アンロック時に発火するイベント
        /// </summary>
        public event Action OnUnlocked;
        /// <summary>
        /// アンロック処理
        /// </summary>
        public void Unlock();
    }
}
