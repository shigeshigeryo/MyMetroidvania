using System.Collections;
using UnityEngine;

public class EnemyWalker : EnemyBase
{
    [Header("UŒ‚iTestEnemyj")]
    [SerializeField, Tooltip("UŒ‚”»’è‚ÌŒ´“_")] private Transform _hitBoxOriginTransform = null;
    [SerializeField, Tooltip("UŒ‚”»’è")] private HitBox _hitBox;
    [SerializeField, Tooltip("UŒ‚CTi•bj")] private float _coolSec = 1f;
    private bool _isAttacking = false;

    private enum State
    {
        Idle, // ‘Ò‹@
        Battle, // í“¬ó‘Ô
        Death // €–S
    }
    private State _currentState = State.Idle;

    public override void Initialize()
    {
        base.Initialize();
        if (gameObject.activeInHierarchy) StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            _isAttacking = true;
            _hitBox.SetEnableCollider();

            yield return new WaitForSeconds(1f);

            _hitBox.SetDisableCollider();
            _isAttacking = false;
            yield return new WaitForSeconds(_coolSec);
        }
    }

    protected override void OnDamageTaken()
    {
        base.OnDamageTaken();
        Debug.Log($"Life:{_statusManager.CurrentStatus.Life}", _statusManager);
    }
}
