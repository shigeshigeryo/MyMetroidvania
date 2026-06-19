using UnityEngine;

namespace MyMetroidVania.Entity.Gimmick.Item
{
    /// <summary>
    /// 最大体力増加アイテムを管理
    /// </summary>
    public class LifeUpItem : ItemBase
    {
        /// <summary>
        /// 取得したときの処理
        /// 取得者の最大体力を増加させる
        /// </summary>
        /// <param name="collision">このアイテムを取得した対象</param>
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
