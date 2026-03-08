using CPlayer = MyMetroidVania.Entity.Character.Player.Player;
using MyMetroidVania.Utility;
using System.Collections;
using UnityEngine;
using MyMetroidVania.Data.ScriptableObjects;
using System.Collections.Generic;
using MyMetroidVania.System;

namespace MyMetroidVania.Entity.Character.Enemy.Walker
{
    public class EnemyWalker : EnemyBase
    {
        private float _lastMoveDirection;// 最後に動いた方向 +x方向 = 1, -x方向 = -1

        [Header("サウンド（Walker）")]
        [SerializeField, Tooltip("着地時音源ファイル名")]
        private string[] _landSoundNames = { "SE_WalkerLand1", "SE_WalkerLand2" };
        private List<SoundData> _landSounds = new();

        [Space]
        [SerializeField, Tooltip("地面の接地判定")] protected BoxCaster _groundChecker = null;
        [SerializeField, Tooltip("プレイヤーチェッカー")] private CircleCaster _playerChecker = null;

        [Header("待機")]
        [SerializeField, Tooltip("x軸の移動の速さ")] protected float _moveSpeedX = 5f;
        [SerializeField, Tooltip("1ループでの徘徊時間")]
        private float _idleDurationSec = 2f;
        [SerializeField, Tooltip("ループで発生するインターバル秒")]
        private float _intervalSec = 1f;
        [SerializeField, Tooltip("時間にランダム性を持たせる最大オフセット値")]
        private const float _offsetSec = 0.5f;

        [Header("追跡")]
        [SerializeField, Tooltip("追跡する際の速さ")] private float _chaseSpeedX = 2.0f;
        private Transform _target;

        [Space]
        [SerializeField] private EnemyWalkerAnimation _animation = null;

        private SleepState _sleepState = null;
        private SleepState SleepState
        {
            get
            {
                if (_sleepState == null)
                {
                    _sleepState = new SleepState(this, new WalkerIdleState(this));
                }

                return _sleepState;
            }
        }

        public override void InitializeOnce()
        {
            base.InitializeOnce();

            for (int i = 0; i < _landSoundNames.Length; i++)
            {
                _landSounds.Add(AudioManager.Instance.GetSe(_landSoundNames[i]));
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            ChangeState(SleepState);

            _animation.OnLand += PlayOneShotLandSound;
            _animation.OnCompleteDeadAnimation += OnCompletedDeadAnimation;
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
        /// 着地サウンドを再生する
        /// どれを再生するかはランダムで決める
        /// </summary>
        private void PlayOneShotLandSound()
        {
            if (_groundChecker.IsCasted)
            {
                var i = Random.Range(0, _landSoundNames.Length);
                _audioSource.PlayOneShot(_landSounds[i].Clip, _landSounds[i].Volume);
            }
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
                _target = hit.GetComponent<CPlayer>().Center;
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
        /// 検知範囲内にいることは前提
        /// </summary>
        public override bool IsPlayerInRange()
        {
            return _playerChecker.GetHitCollider();
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
            _rb.linearVelocity = Vector2.zero;
        }

        /*
         * ------------------------------------------------------------------
         * バトルステートのアクションを制御
         * ------------------------------------------------------------------
         */

        /// <summary>
        /// walkerはアタックを使用しない
        /// TODO：仕様確定次第削除
        /// </summary>
        public override IEnumerator OnAttack()
        {
            yield return null;
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
            _animation.OnLand -= PlayOneShotLandSound;
            _animation.OnCompleteDeadAnimation -= OnCompletedDeadAnimation;
        }
    }
}