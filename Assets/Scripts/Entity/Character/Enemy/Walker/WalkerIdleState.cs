using System.Collections;
using UnityEngine;

/// <summary>
/// ウォーカーの待機ステートを管理
/// </summary>
public class WalkerIdleState : EnemyState
{
    [SerializeField, Tooltip("アイドル状態を持続する時間（秒）")] private float _durationSec = 0.5f;
    private int _playerLayer;
    public WalkerIdleState(EnemyBase enemy) : base(enemy) 
    {
        _playerLayer = LayerMask.GetMask("Player");
    }

    public override IEnumerator Execute()
    {
        Debug.Log("ウォーカーの待機ステート行動中");
        yield return new WaitForSeconds(_durationSec);
    }
}
