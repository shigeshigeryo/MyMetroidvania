using System.Threading;
using UnityEngine;

/// <summary>
/// ウォーカーのバトルステートを管理
/// </summary>
public class WalkerBattleState : EnemyState
{
    public WalkerBattleState(EnemyBase enemy) : base(enemy) { }

    /// <summary>
    /// ステートの状態遷移を監視
    /// </summary>
    protected override void OnTick()
    {
        Debug.Log("ウォーカーのバトルステート行動中");
        if (!_owner.IsPlayerInRange())
        {
            // プレイヤーが攻撃射程内にいない
            _owner.ChangeState(new WalkerChaseState(_owner));
        }
    }
}
