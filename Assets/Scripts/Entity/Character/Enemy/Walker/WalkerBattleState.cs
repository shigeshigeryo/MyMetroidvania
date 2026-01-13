using System.Collections;
using UnityEngine;

/// <summary>
/// ウォーカーのバトルステートを管理
/// </summary>
public class WalkerBattleState : EnemyState<EnemyWalker>
{ 
    private bool _isAttacking = false;
    public WalkerBattleState(EnemyWalker enemy) : base(enemy) { }

    /// <summary>
    /// ステートに遷移時に発火
    /// </summary>
    public override void Enter()
    {
        _isAttacking = false;
        routine = _owner.StartCoroutine(BattleRoutine());
    }

    /// <summary>
    /// ステートの状態遷移を監視
    /// </summary>
    protected override void OnTick()
    {
        Debug.Log("ウォーカーのバトルステート行動中");
        if (!_isAttacking && !_owner.IsPlayerInRange())
        {
            // プレイヤーが攻撃射程内にいない
            _owner.ChangeState(new WalkerChaseState(_owner));
        }
    }

    /// <summary>
    /// 次のステートに遷移する前に発火
    /// </summary>
    public override void Exit()
    {
        _owner.StopCoroutine(routine);
    }

    private IEnumerator BattleRoutine()
    {
        while (true)
        {
            yield return null;
        }
    }
}
