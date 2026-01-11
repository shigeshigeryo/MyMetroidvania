using System.Collections;
using UnityEngine;

/// <summary>
/// ウォーカーのバトルステートを管理
/// </summary>
public class WalkerBattleState : EnemyState
{
    [SerializeField, Tooltip("アイドル状態を持続する時間（秒）")] private float _durationSec = 0.5f;

    public WalkerBattleState(EnemyBase enemy) : base(enemy) { }

    public override IEnumerator Execute()
    {
        Debug.Log("ウォーカーのバトルステート行動中");
        yield return new WaitForSeconds(_durationSec);
    }
}
