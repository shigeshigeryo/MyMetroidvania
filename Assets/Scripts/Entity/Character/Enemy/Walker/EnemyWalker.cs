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

    private enum State
    {
        Idle, // 待機
        Chase, // 追跡
        Battle, // 戦闘状態
        Stun, // スタン
        Death // 死亡
    }
    private State _currentState = State.Idle;

    private Coroutine _currentStateRoutine = null;

    public override void Initialize()
    {
        base.Initialize();
        if (gameObject.activeInHierarchy) StartCoroutine(MainStateMachine());
    }

    private IEnumerator MainStateMachine()
    {
        // それぞれのStateを管理するクラス
        EnemyState idleState = new WalkerIdleState(this);
        EnemyState chaseState = new WalkerChaseState(this);
        EnemyState battleState = new WalkerBattleState(this);

        while (true)
        {
            EnemyState selectedState = null;

            switch (_currentState)
            {
                case State.Idle:
                    selectedState = idleState;
                    break;

                case State.Chase:
                    selectedState = chaseState;
                    break;

                case State.Battle:
                    selectedState = battleState;
                    break;

                case State.Stun:
                    // Stun 待機した後に復帰
                    yield return new WaitForSeconds(1.0f);
                    break;
            }

            // ステートが選択されていたら実行
            if (selectedState != null)
            {
                yield return StartCoroutine(RunStateRoutine(selectedState));
            }

            yield return null;
        }
    }

    /// <summary>
    /// 指定のステートのAIを走らせる
    /// </summary>
    /// <param name="stateClass"></param>
    /// <returns></returns>
    private IEnumerator RunStateRoutine(EnemyState stateClass)
    {
        if (stateClass != null)
        {
            _currentStateRoutine = StartCoroutine(stateClass.Execute());

            yield return _currentStateRoutine;

            _currentStateRoutine = null;
        }
        else
        {
            // 指定のステートがない場合（設定ミスなど）の安全策
            yield return new WaitForSeconds(1.0f);
        }

        // プレイヤーとの距離を計算しステートを変更
        _currentState = GetNextState();
    }

    /// <summary>
    /// 検知範囲内のプレイヤーの存在をチェックし、次のステートを取得する
    /// 検知範囲外 → Idle
    /// 攻撃射程内 → Battle
    /// 攻撃射程外 → Chase
    /// </summary>
    private State GetNextState()
    {
        // プレイヤーが検知範囲内にいるか確認
        var hit = _playerChecker.GetHitCollider();
        if (!hit)
        {
            // 検知しない場合は待機
            return State.Idle;
        }

        // 以下検知した場合
        var distance = transform.position - hit.transform.position;
        Debug.Log($"検知範囲内のプレイヤーとの距離の2乗{distance.sqrMagnitude}", this);
        if (distance.sqrMagnitude <= SqrAttackRange)
        {
            // 攻撃射程内の場合は戦闘
            return State.Battle;
        }
        else
        {
            // 攻撃射程外の場合は追跡
            return State.Chase;
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
        // 一度実行中のコルーチンを止める。
        StopAllCoroutines();

        _currentState = State.Stun;
        Debug.Log($"Life:{_statusManager.CurrentStatus.Life}", _statusManager);

        // メインのステートマシンの再起動
        StartCoroutine(MainStateMachine());
    }
}
