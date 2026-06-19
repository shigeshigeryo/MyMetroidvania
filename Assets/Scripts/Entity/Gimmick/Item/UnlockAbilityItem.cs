using MyMetroidVania.Entity.Character.Player;
using UnityEngine;

namespace MyMetroidVania.Entity.Gimmick.Item
{
    /// <summary>
    /// アビリティをアンロックするアイテムを管理
    /// </summary>
    public class UnlockAbilityItem : ItemBase
    {
        [SerializeField, Tooltip("アンロックするアビリティ")] private AbilityType _unlockAbilityType;

        /// <summary>
        /// 取得したときの処理
        /// アビリティをアンロックする
        /// </summary>
        /// <param name="collision">このアイテムを取得した対象</param>
        protected override void Apply(Collider2D collision)
        {
            _currentState = State.PickedUpUnique;
            _stateData.SetState((int)_currentState);

            if (collision.TryGetComponent<Player>(out var player))
            {
                player.UnlockAbility(_unlockAbilityType);
            }
        }
    }
}
