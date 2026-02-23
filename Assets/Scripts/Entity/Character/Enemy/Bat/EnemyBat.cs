using MyMetroidVania.Utility;
using System.Collections;
using UnityEngine;

namespace MyMetroidVania.Entity.Character.Enemy.Bat
{
    public class EnemyBat : EnemyBase
    {
        private float _lastMoveDirection;// 最後に動いた方向 +x方向 = 1, -x方向 = -1

        [Header("攻撃（TestEnemy）")]
        [SerializeField, Tooltip("攻撃判定")] private HitBox _hitBox;
        [SerializeField, Tooltip("攻撃の射程")] private float _attackRange = 1f;
        private float SqrAttackRange => _attackRange * _attackRange; // 攻撃の射程の2乗
        [SerializeField, Tooltip("攻撃時の体当たりの力")] private float _power = 5f;
        [SerializeField, Tooltip("攻撃CT（秒）")] private float _coolSec = 1f;

        [SerializeField, Tooltip("プレイヤーチェッカー")] private CircleCaster _playerChecker = null;

        [Header("待機")]
        [SerializeField, Tooltip("移動速度")] private float _moveSpeed = 0.9f;

        [Header("追跡")]
        [SerializeField, Tooltip("追跡する際の速さ")] private float _chaseSpeed = 2.0f;
        private Transform _target;

        [Space]
        [SerializeField] private EnemyBatAnimation _animation = null;

        private SleepState _sleepState = null;
        private SleepState SleepState
        {
            get
            {
                if (_sleepState == null)
                {
                    _sleepState = new SleepState(this, new BatIdleState(this));
                }

                return _sleepState;
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            ChangeState(SleepState);

            _animation.OnAttack += Charge;
            _animation.OnCompleteDeadAnimation += OnCompletedDeadAnimation;
        }

        public override void Respawn()
        {
            base.Respawn();
            ChangeState(SleepState);
        }

        private void FixedUpdate()
        {
            _animation.UpdateParam(_rb.linearVelocity);
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
        /// 元の場所に戻る
        /// </summary>
        public override IEnumerator OnIdle()
        {
            // 初期位置に戻る
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
            Vector2 val = dir.normalized * _moveSpeed;
            _lastMoveDirection = val.x >= 0 ? 1 : -1; 

            while (true)
            {
                if ((_initialPosition - transform.position).sqrMagnitude < 1) break;

                _rb.linearVelocity = val;
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
         * 追跡ステートのアクションを制御
         * ------------------------------------------------------------------
         */
        /// <summary>
        /// 追跡開始
        /// </summary>
        public void StartChase()
        {
            // 追跡開始時の処理を書く
        }

        /// <summary>
        /// 追跡する
        /// </summary>
        public override IEnumerator OnChase()
        {
            var dir = (_target.position - transform.position).normalized;
            _lastMoveDirection = dir.x < 0 ? -1 : 1;
            _rb.linearVelocity = dir * _chaseSpeed;
            yield return new WaitForFixedUpdate();
        }

        /// <summary>
        /// 追跡を停止
        /// </summary>
        public override void StopChase()
        {
            _rb.linearVelocity = Vector2.zero;
        }

        /*
         * ------------------------------------------------------------------
         * バトルステートのアクションを制御
         * ------------------------------------------------------------------
         */
        public override IEnumerator OnAttack()
        {
            // 攻撃開始
            _animation.TriggerAttack();
            yield return new WaitForSeconds(0.5f);

            // 攻撃終了後の隙
            _hitBox.SetDisableCollider();
            StopMove();
            yield return new WaitForSeconds(_coolSec);
        }

        /// <summary>
        /// 突撃挙動
        /// </summary>
        private void Charge()
        {
            _hitBox.SetEnableCollider();

            var dir = (_target.position - transform.position).normalized;
            _rb.AddForce(dir * _power, ForceMode2D.Impulse);
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
            _animation.OnAttack -= Charge;
            _animation.OnCompleteDeadAnimation -= OnCompletedDeadAnimation;
        }
    }
}