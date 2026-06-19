using MyMetroidVania.Data;
using MyMetroidVania.System;
using System;
using UnityEngine;

namespace MyMetroidVania.Entity.Character.Player
{
    /// <summary>
    /// アビリティタイプ
    /// ビットフラグで管理する
    /// </summary>
    [Flags]
    public enum AbilityType
    {
        None = 0,
        Hook = 1 << 0,
    }

    /// <summary>
    /// アビリティのアンロック、習得状況の管理
    /// </summary>
    public class AbilityManager : MonoBehaviour
    {
        private TargetStateData _stateData;

        [SerializeField]
        private AbilityType _unlockedAbilities;

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Start()
        {
            // WorldStateDataはAwakeで取得済み
            WorldManager.Instance.WorldStateData.TryGetAllAreaTargetState("Ability", out _stateData);
            _unlockedAbilities = (AbilityType)_stateData.State;
        }

        /// <summary>
        /// アビリティをアンロックする
        /// </summary>
        /// <param name="type">アンロックするアビリティ</param>
        public void UnlockAbility(AbilityType type)
        {
            _unlockedAbilities |= type;
            _stateData.SetState((int)_unlockedAbilities);
        }

        /// <summary>
        /// アビリティを習得しているかどうかを取得する
        /// </summary>
        /// <param name="type">確認したいアビリティ</param>
        /// <returns>習得していればtrue</returns>
        public bool HasAbility(AbilityType type)
        {
            return (_unlockedAbilities & type) == type;
        }
    }
}
