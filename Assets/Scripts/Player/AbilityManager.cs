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
    [SerializeField]
    private AbilityType _unlockedAbilities;
    public AbilityType UnlockedAbilities => _unlockedAbilities;

    public void UnlockAbility(AbilityType type)
    {
        _unlockedAbilities |= type;
    }

    public bool HasAbility(AbilityType type)
    {
        return (UnlockedAbilities & type) == type;
    }
}
