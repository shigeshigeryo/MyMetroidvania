using UnityEngine;
using UnityEngine.Pool;

public class Effect : MonoBehaviour
{
    private IObjectPool<Effect> _pool;

    public void SetPool(IObjectPool<Effect> pool)
    {
        _pool = pool;
    }

    /// <summary>
    /// アニメーションが終わったら発火する　※アニメーションイベントで設定
    /// プールに戻す処理
    /// </summary>
    public void OnFinished()
    {
        _pool.Release(this);
    }
}
