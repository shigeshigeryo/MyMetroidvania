using MyMetroidVania.Utility;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace MyMetroidVania.Entity.Character.Enemy.Slime
{
    public class EnemySlime : EnemyBase
    {
        private float _lastMoveDirection;// 最後に動いた方向 +x方向 = 1, -x方向 = -1

        [Header("待機（TestEnemy）")]
        [SerializeField, Tooltip("x軸の移動の速さ")] protected float _moveSpeedX = 5f;
        [SerializeField, Tooltip("ループで発生するインターバル秒")]
        private float _intervalSec = 1f;
        [SerializeField, Tooltip("時間にランダム性を持たせる最大オフセット値")]
        private const float _offsetSec = 0.5f;

        [Header("近距離バトルステート")]
        [SerializeField, Tooltip("攻撃判定")] private HitBox _hitBox;
        [SerializeField, Tooltip("攻撃の射程")] private float _attackRange = 5f;
        private float SqrAttackRange => _attackRange * _attackRange; // 攻撃の射程の2乗
        [SerializeField, Tooltip("攻撃CT（秒）")] private float _coolSec = 1f;
        [SerializeField, Tooltip("衝撃波")] private ShockWave _shockWavePrefab = null;
        private IObjectPool<ShockWave> _shockWavePool;
        [SerializeField, Tooltip("衝撃波の広がる数")] private int _shockWaveCount = 4;
        [SerializeField, Tooltip("次の衝撃波が出現する時間")] private float _shockWaveInterval = 0.3f;

        [SerializeField, Tooltip("プレイヤーチェッカー")] private CircleCaster _playerChecker = null;

        [Header("遠距離バトルステート")]
        [SerializeField, Tooltip("逃げる挙動の最大持続時間（秒）")] private float _timeLimit = 2f;
        private Transform _target;

        [Space]
        [SerializeField] private EnemySlimeAnimation _animation = null;

        private SleepState _sleepState = null;
        private SleepState SleepState
        {
            get
            {
                if(_sleepState == null)
                {
                    _sleepState = new SleepState(this, new SlimeIdleState(this));
                }

                return _sleepState;
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            _hitBox.SetDisableCollider();
            ChangeState(SleepState);

            _animation.OnAttack += CrushUp;
            _animation.OnAbility += UseAbility;

            // 衝撃波のプールを生成
            _shockWavePool = new ObjectPool<ShockWave>(
                createFunc: () =>
                {
                    ShockWave shockWave = Instantiate(_shockWavePrefab);
                    shockWave.Initialize(_statusManager);
                    shockWave.SetPool(_shockWavePool);
                    return shockWave;
                },
                actionOnGet: (shockWave) =>
                {
                    shockWave.gameObject.SetActive(true);
                },
                actionOnRelease: (shockWave) =>
                {
                    shockWave.gameObject.SetActive(false);
                },
                actionOnDestroy: (shockWave) =>
                {
                    Destroy(shockWave.gameObject);
                },
                defaultCapacity: 4, // 準備数（仮）
                maxSize: _shockWaveCount*2 // 最大数（仮）
            );
        }

        public override void Respawn()
        {
            base.Respawn();
            ChangeState(SleepState);
        }

        private void FixedUpdate()
        {
            _animation.UpdateParam(_rb.linearVelocityX);
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
            return distance.sqrMagnitude <= SqrAttackRange;
        }

        /*
         * ------------------------------------------------------------------
         * 待機ステートのアクションを制御
         * ------------------------------------------------------------------
         */
        /// <summary>
        /// 徘徊する
        /// </summary>
        public override IEnumerator OnIdle()
        {
            yield return ReturnRoutine();
            StopMove();
        }

        /// <summary>
        /// 移動を開始
        /// </summary>
        private IEnumerator ReturnRoutine()
        {
            // 移動方向を決定
            var dir = _initialPosition - transform.position;
            _lastMoveDirection = dir.x >= 0 ? 1 : -1;

            while (true)
            {
                if ((_initialPosition - transform.position).sqrMagnitude < 1) break;

                _rb.linearVelocityX = _lastMoveDirection * _moveSpeedX;
                yield return new WaitForFixedUpdate();
            }
        }

        /// <summary>
        /// 徘徊を停止
        /// </summary>
        public override void StopMove()
        {
            _rb.linearVelocity = Vector2.zero;
        }

        /*
         * ------------------------------------------------------------------
         * 近距離バトルステートのアクションを制御
         * ------------------------------------------------------------------
         */
        public override IEnumerator OnAttack()
        {
            // 攻撃開始
            _animation.TriggerAttack();
            yield return new WaitForSeconds(1f);

            // 攻撃終了後の隙
            _hitBox.SetDisableCollider();
            yield return new WaitForSeconds(_coolSec);
        }

        /// <summary>
        /// 押しつぶす挙動と衝撃波の生成の生成を発火
        /// </summary>
        private void CrushUp()
        {
            _hitBox.SetEnableCollider();
            StartCoroutine(GenerateShockWave());
        }

        /// <summary>
        /// 衝撃波の生成
        /// </summary>
        private IEnumerator GenerateShockWave()
        {
            for (int i = 1; i <= _shockWaveCount; i++)
            {
                // 両側に広がるように衝撃波の生成
                var leftWave = _shockWavePool.Get();
                leftWave.transform.position = transform.position - Vector3.right * i;
                leftWave.SetFlipX(false);

                var rightWave = _shockWavePool.Get();
                rightWave.transform.position = transform.position + Vector3.right * i;
                rightWave.SetFlipX(true);

                yield return new WaitForSeconds(_shockWaveInterval);
            }
        }

        /// <summary>
        /// 追跡する
        /// 遠距離ステートでも使用する
        /// </summary>
        public override IEnumerator OnChase()
        {
            Debug.Log("スライムが近づくよ");
            var dir = _target.position - transform.position;
            _lastMoveDirection = dir.x < 0 ? -1 : 1;
            float timer = 0f;

            while (timer < _timeLimit)
            {
                if ((_target.position - transform.position).sqrMagnitude < SqrAttackRange) yield break;

                _rb.linearVelocityX = _lastMoveDirection * _moveSpeedX;
                yield return new WaitForFixedUpdate();
                timer += Time.fixedDeltaTime;
            }
        }

        /// <summary>
        /// 追跡を停止
        /// </summary>
        public override void StopChase()
        {
            _rb.linearVelocity = Vector2.zero;
        }

        /// <summary>
        /// プレイヤーから離れる挙動
        /// </summary>
        public IEnumerator OnEscape()
        {
            Debug.Log("スライムが離れるよ");
            var dir = _target.position - transform.position;
            // 逃げる挙動のため、ベクトルとは逆向きをとる
            _lastMoveDirection = dir.x < 0 ? 1 : -1;
            float timer = 0f;

            while (timer < _timeLimit)
            {
                if (!IsPlayerInRange()) yield break;
                
                _rb.linearVelocityX = _lastMoveDirection * _moveSpeedX;
                yield return new WaitForFixedUpdate();
                timer += Time.fixedDeltaTime;
            }
        }

        /*
         * ------------------------------------------------------------------
         * 遠距離バトルステートのアクションを制御
         * ------------------------------------------------------------------
         */
        public IEnumerator OnAbility()
        {
            // 攻撃開始
            _animation.TriggerAbility();
            yield return new WaitForSeconds(1f);

            // 攻撃終了後の隙
            yield return new WaitForSeconds(_coolSec);
        }

        /// <summary>
        /// アビリティ処理
        /// </summary>
        private void UseAbility()
        {
            Debug.Log("アビリティを処理");
        }

        /*
         * ------------------------------------------------------------------
         * リアクションを制御
         * ------------------------------------------------------------------
         */
        protected override void OnTakenDamage()
        {
            base.OnTakenDamage();
            Debug.Log($"Life:{_statusManager.CurrentStatus.Life}", _statusManager);
        }

        private void OnDisable()
        {
            _animation.OnAttack -= CrushUp;
            _animation.OnAbility -= UseAbility;
        }
    }
}