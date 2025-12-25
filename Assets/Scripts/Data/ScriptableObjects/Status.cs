using UnityEngine;

[CreateAssetMenu(fileName = "Status", menuName = "Data/Status")]
public class Status : ScriptableObject
{
    [SerializeField, Tooltip("ƒ‰ƒCƒt")] private int _life;
    public int Life => _life;
    [SerializeField, Tooltip("UŒ‚—Í")] private int _attackPower;
    public int AttackPower => _attackPower;
}
