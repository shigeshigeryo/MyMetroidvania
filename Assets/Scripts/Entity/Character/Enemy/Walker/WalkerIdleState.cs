using System.Collections;
using UnityEngine;

/// <summary>
/// ウォーカーの待機ステートを管理
/// </summary>
public class WalkerIdleState : EnemyState<EnemyWalker>
{
    private float _idleDurationSec = 1f;

    // 遷移先候補のステート
    public WalkerIdleState(EnemyWalker enemy) : base(enemy) 
    {
        _idleDurationSec = enemy.IdleDurationSec;
    }

    public override void Enter()
    {
        routine = _owner.StartCoroutine(IdleRoutine());
    }

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

    public override void Exit()
    {
        _owner.StopCoroutine(routine);
    }

    /// <summary>
    /// 待機ステート中の行動
    /// ・徘徊
    /// </summary>
    /// <returns></returns>
    private IEnumerator IdleRoutine()
    {
        while (true)
        {
            _owner.Move();
            yield return new WaitForSeconds(_idleDurationSec);
            _owner.StopMove();
            yield return new WaitForSeconds(_idleDurationSec);
        }
    }
}
