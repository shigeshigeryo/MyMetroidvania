using System.Collections;
using UnityEngine;

public class EnemyWalker : EnemyBase
{
    private float _lastMoveDirection;// 最後に動いた方向 +x方向 = 1, -x方向 = -1

    [Header("攻撃（TestEnemy）")]
    [SerializeField, Tooltip("攻撃判定")] private HitBox _hitBox;
    [SerializeField, Tooltip("攻撃の射程")] private float _attackRange = 1f;
    private float SqrAttackRange => _attackRange * _attackRange; // 攻撃の射程の2乗
    [SerializeField, Tooltip("攻撃CT（秒）")] private float _coolSec = 1f;

    [SerializeField, Tooltip("プレイヤーチェッカー")] private CircleCaster _playerChecker = null;

    [Header("待機")]
    [SerializeField, Tooltip("1ループでの徘徊時間")]
    private float _idleDurationSec = 2f;
    [SerializeField, Tooltip("ループで発生するインターバル秒")]
    private float _intervalSec = 1f;
    [SerializeField, Tooltip("時間にランダム性を持たせる最大オフセット値")]
    private const float _offsetSec = 0.5f;

    [Header("追跡")]
    [Tooltip("追跡する際の速さ")] private float _chaseSpeedX = 2.0f;

    private Coroutine _currentRoutine = null; // 現在のコルーチン

    private Transform _target;

    public override void Initialize()
    {
        base.Initialize();
        ChangeState(new SleepState(this, new WalkerIdleState(this)));
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
            _target = hit.transform;
            return true;
        }
        else
        {
            _target = null;
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

    /// <summary>
    /// 最後に移動した方向を参照して攻撃判定のポジションをセットする
    /// X軸のみを移動
    /// </summary>
    /// <param name="dir">X軸速度</param>
    private void SetAttackColliderByLastMoveDirection()
    {
        // 引数は攻撃の射程と向いている向きを考慮したもの
        _hitBox.SetPosition(Vector3.right * _lastMoveDirection * _attackRange / 2);
    }

    /*
     * ------------------------------------------------------------------
     * 待機ステートのアクションを制御
     * ------------------------------------------------------------------
     */
    /// <summary>
    /// 徘徊する
    /// </summary>
    public override IEnumerator OnMove()
    {
        // 一定時間徘徊する
        Coroutine walkRoutine = StartCoroutine(WalkRoutine());
        float offset = Random.Range(-_offsetSec, _offsetSec);
        yield return new WaitForSeconds(_idleDurationSec + offset);

        // 一定時間のインターバル
        StopCoroutine(walkRoutine);
        offset = Random.Range(-_offsetSec, _offsetSec);
        yield return new WaitForSeconds(_intervalSec + offset);        
    }

    /// <summary>
    /// 移動を開始
    /// </summary>
    /// <returns></returns>
    private IEnumerator WalkRoutine()
    {
        // 移動方向を決定
        _lastMoveDirection = Random.Range(0, 2) == 0 ? -1 : 1;
        float val = _lastMoveDirection * _moveSpeedX;

        while (true)
        {
            _rb.linearVelocityX = val;
            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// 徘徊を停止
    /// </summary>
    public override void StopMove()
    {
        _rb.linearVelocityX = 0;
    }


    /*
     * ------------------------------------------------------------------
     * 追跡ステートのアクションを制御
     * ------------------------------------------------------------------
     */
    /// <summary>
    /// 追跡する
    /// </summary>
    public override IEnumerator OnChase()
    {
        var dir = _target.position - transform.position;
        _lastMoveDirection = dir.x < 0 ? -1 : 1;
        _rb.linearVelocityX = _lastMoveDirection * _chaseSpeedX;
        yield return new WaitForFixedUpdate();
    }

    /// <summary>
    /// 追跡を停止
    /// </summary>
    public override void StopChase()
    {
        _rb.linearVelocity = Vector3.zero;
    }

    /*
     * ------------------------------------------------------------------
     * バトルステートのアクションを制御
     * ------------------------------------------------------------------
     */
    public override IEnumerator OnAttack()
    {
        // 攻撃方向をセット
        SetAttackColliderByLastMoveDirection();

        // 攻撃開始
        _hitBox.SetEnableCollider();
        yield return new WaitForSeconds(1f);

        // 攻撃終了後の隙
        _hitBox.SetDisableCollider();
        yield return new WaitForSeconds(_coolSec);
    }


    /*
     * ------------------------------------------------------------------
     * リアクションを制御
     * ------------------------------------------------------------------
     */
    protected override void OnDamageTaken()
    {
        base.OnDamageTaken();
        Debug.Log($"Life:{_statusManager.CurrentStatus.Life}", _statusManager);
    }
}
