using MyMetroidVania.Entity.Character.Player;

namespace MyMetroidVania.Entity.Gimmick
{
    /// <summary>
    /// インタラクト可能なクラスに実装するインタフェース
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// インタラクト時の処理
        /// </summary>
        /// <param name="player">プレイヤー</param>
        public void Interact(Player player);
    }
}
