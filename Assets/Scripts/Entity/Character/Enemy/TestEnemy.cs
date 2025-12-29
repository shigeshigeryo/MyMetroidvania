using UnityEngine;

public class TestEnemy : EnemyBase
{
    protected void Start()
    {
        Initialize();
        InitializeEvents();
    }

    protected override void Damaged()
    {
        base.Damaged();
        Debug.Log($"Life:{_statusManager.CurrentStatus.Life}");
    }
}
