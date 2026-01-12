using System.Collections;
using UnityEngine;

/// <summary>
/// ウォーカーの待機ステートを管理
/// </summary>
public class WalkerIdleState : EnemyState<EnemyWalker>
{
    private float _idleDurationSec = 1f; // 徘徊時間
    private float _intervalSec = 0.5f; // 1ループのインターバル秒
    private const float OFFSET_SEC = 0.5f; // 時間にランダム性を持たせる最大オフセット値

    // 遷移先候補のステート
    public WalkerIdleState(EnemyWalker enemy) : base(enemy) 
    {
        _idleDurationSec = enemy.IdleDurationSec;
        _intervalSec = enemy.IntervalSec;

        Debug.Log(_idleDurationSec);
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
        _owner.StopMove();
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
            Debug.Log("loop");
            
            _owner.Move();
            float offset = Random.Range(-OFFSET_SEC, OFFSET_SEC);
            yield return new WaitForSeconds(_idleDurationSec + offset);
            
            _owner.StopMove();
            offset = Random.Range(-OFFSET_SEC, OFFSET_SEC);
            yield return new WaitForSeconds(_intervalSec + offset);
        }
    }
}
