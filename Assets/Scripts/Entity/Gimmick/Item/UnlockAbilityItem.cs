using MyMetroidVania.Entity.Character.Player;
using UnityEngine;

namespace MyMetroidVania.Entity.Gimmick.Item
{
    public class UnlockAbilityItem : ItemBase
    {
        [SerializeField, Tooltip("アンロックするアビリティ")] private AbilityType _unlockAbilityType;

        /// <summary>
        /// ItemBaseのOnTriggerEnter2D時に発火
        /// アビリティをアンロックする
        /// </summary>
        /// <param name="collision"></param>
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