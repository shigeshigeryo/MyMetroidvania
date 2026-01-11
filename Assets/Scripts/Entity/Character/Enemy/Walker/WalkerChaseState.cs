using System.Collections;
using UnityEngine;

public class WalkerChaseState : EnemyState
{
    [SerializeField, Tooltip("アイドル状態を持続する時間（秒）")] private float _durationSec = 0.5f;

    public WalkerChaseState(EnemyBase enemy) : base(enemy) { }

    public override IEnumerator Execute()
    {
        Debug.Log("ウォーカーの追跡ステート行動中");
        yield return new WaitForSeconds(_durationSec);
    }
}
