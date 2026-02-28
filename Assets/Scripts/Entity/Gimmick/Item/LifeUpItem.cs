using UnityEngine;

namespace MyMetroidVania.Entity.Gimmick.Item
{
    public class LifeUpItem : ItemBase
    {
        protected override void Apply(Collider2D collision)
        {
            _currentState = State.PickedUpUnique;
            _stateData.SetState((int)_currentState);

            if (collision.TryGetComponent<StatusManager>(out var status))
            {
                status.LifeUp();
            }
        }
    }
}