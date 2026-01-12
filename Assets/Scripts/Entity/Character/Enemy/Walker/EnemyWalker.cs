using System.Collections;
using UnityEngine;

public class EnemyWalker : EnemyBase
{
    [Header("攻撃（TestEnemy）")]
    [SerializeField, Tooltip("攻撃判定の原点")] private Transform _hitBoxOriginTransform = null;
    [SerializeField, Tooltip("攻撃判定")] private HitBox _hitBox;
    [SerializeField, Tooltip("攻撃の射程")] private float _attackRange = 1f;
    private float SqrAttackRange => _attackRange * _attackRange; // 攻撃の射程の2乗
    [SerializeField, Tooltip("攻撃CT（秒）")] private float _coolSec = 1f;
    private bool _isAttacking = false;

    [SerializeField, Tooltip("プレイヤーチェッカー")] private CircleCaster _playerChecker = null;

    public override void Initialize()
    {
        base.Initialize();
        if (gameObject.activeInHierarchy) ChangeState(new WalkerIdleState(this));
    }


    /// <summary>
    /// 検知範囲内にプレイヤーが存在するか返す
    /// </summary>
    public override bool IsPlayerDetected()
    {
        // プレイヤーが検知範囲内にいるか確認
        var hit = _playerChecker.GetHitCollider();
        if (hit)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 攻撃射程にプレイヤーが存在するか返す
    /// </summary>
    public override bool IsPlayerInRange()
    {
        var hit = _playerChecker.GetHitCollider();
        if (!hit)
        {
            return false;
        }

        // 以下検知した場合
        var distance = transform.position - hit.transform.position;
        if (distance.sqrMagnitude <= SqrAttackRange)
        {
            // 攻撃射程内
            return true;
        }
        else
        {
            // 攻撃射程外
            return false;
        }
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
