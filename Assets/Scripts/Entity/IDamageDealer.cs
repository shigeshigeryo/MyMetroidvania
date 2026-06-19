namespace MyMetroidVania.Entity
{
    /// <summary>
    /// ダメージを与えるものに実装するインタフェース
    /// </summary>
    public interface IDamageDealer
    {
        /// <summary>
        /// 攻撃力を取得する
        /// </summary>
        /// <returns>攻撃力の値</returns>
        public int GetAttackPower();
    }
}
