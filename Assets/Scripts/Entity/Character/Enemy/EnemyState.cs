using UnityEngine;

/// <summary>
/// エネミーの状態を管理する基底クラス
/// </summary>
public abstract class EnemyState
{
    protected Coroutine routine;
    protected float _timer = 0;
    protected float _tickIntervalSec = 0.25f; // Tickインターバル秒

    /// <summary>
    /// ステートに遷移時に発火
    /// </summary>
    public virtual void Enter() { }
    /// <summary>
    /// ステートの監視を制御
    /// Update内で発火
    /// </summary>
    public void Tick()
    {
        // 遷移の監視にインターバルを設ける
        if (_timer < _tickIntervalSec)
        {
            _timer += Time.deltaTime;
            return;
        }
        _timer = 0; // タイマーリセット

        OnTick();
    }
    protected abstract void OnTick();
    /// <summary>
    /// 次のステートに遷移する前に発火
    /// </summary>
    public virtual void Exit() { }
}

/// <summary>
/// _ownerの型情報を具体化するために実装
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class EnemyState<T> : EnemyState where T : EnemyBase
{
    protected T _owner;
    public EnemyState(T enemy)
    {
        _owner = enemy;
    }
}
