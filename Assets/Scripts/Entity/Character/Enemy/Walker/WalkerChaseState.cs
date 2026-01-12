using UnityEngine;

/// <summary>
/// ウォーカーの追跡ステートを管理
/// </summary>
public class WalkerChaseState : EnemyState<EnemyWalker>
{
    public WalkerChaseState(EnemyWalker enemy) : base(enemy){ }

    /// <summary>
    /// ステートの状態遷移を監視
    /// </summary>
    protected override void OnTick()
    {
        Debug.Log("ウォーカーの追跡ステート行動中");
        if (!_owner.IsPlayerDetected())
        {
            // プレイヤーが検知外に出た
            _owner.ChangeState(new WalkerIdleState(_owner));
            return;
        }

        if (_owner.IsPlayerInRange())
        {
            // プレイヤーが攻撃射程内にいる
            _owner.ChangeState(new WalkerBattleState(_owner));
            return;
        }
    }
}
