using System.Collections;
using UnityEngine;

/// <summary>
/// エネミーの状態を管理する基底クラス
/// </summary>
public abstract class EnemyState
{
    protected EnemyBase _owner;
    protected Coroutine routine;

    public EnemyState(EnemyBase enemy)
    {
        _owner = enemy;
    }

    public abstract IEnumerator Execute();
}
