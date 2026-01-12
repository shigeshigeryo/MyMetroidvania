using UnityEngine;

/// <summary>
/// ウォーカーの待機ステートを管理
/// </summary>
public class WalkerIdleState : EnemyState
{
    // 遷移先候補のステート
    public WalkerIdleState(EnemyBase enemy) : base(enemy) { }

    /// <summary>
    /// ステートの状態遷移を監視
    /// </summary>
    protected override void OnTick()
    {
        Debug.Log("ウォーカーの待機ステート行動中");
        if (_owner.IsPlayerDetected())
        {
            // プレイヤーを検知した
            _owner.ChangeState(new WalkerChaseState(_owner));
        }
    }
}
