using System.Collections;
using UnityEngine;

/// <summary>
/// ウォーカーの待機ステートを管理
/// </summary>
public class WalkerIdleState : EnemyState<EnemyWalker>
{
    public WalkerIdleState(EnemyWalker enemy) : base(enemy) { }

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
            return;
        }

        if (!_owner.IsVisible())
        {
            // 画面外に出た
            _owner.ChangeState(new SleepState(_owner, this));
            return;
        }
    }

    public override void Exit()
    {
        _owner.StopCoroutine(routine);
        _owner.StopMove();
    }


    private IEnumerator IdleRoutine()
    {
        while (true)
        {
            yield return _owner.OnMove();
        }
    }
}
