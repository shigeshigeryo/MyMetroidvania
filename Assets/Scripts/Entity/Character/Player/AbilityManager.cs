using UnityEngine;

// ビットフラグで管理する
[System.Flags]
public enum AbilityType
{
    None = 0,
    Hook = 1 << 0,
}

// アビリティのアンロック、習得状況の管理
public class AbilityManager : MonoBehaviour
{
    private TargetStateData _stateData;

    [SerializeField]
    private AbilityType _unlockedAbilities;

    public void Start()
    {
        // WorldStateDataはAwakeで取得済み
        WorldManager.Instance.WorldStateData.TryGetAllAreaTargetState("Ability", out _stateData);
        _unlockedAbilities = (AbilityType)_stateData.State;
    }

    public void UnlockAbility(AbilityType type)
    {
        _unlockedAbilities |= type;
        _stateData.SetState((int)_unlockedAbilities);
    }

    public bool HasAbility(AbilityType type)
    {
        return (_unlockedAbilities & type) == type;
    }
}
