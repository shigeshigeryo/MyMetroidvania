using MyMetroidVania.Data.ScriptableObjects;
using MyMetroidVania.System;
using System;
using System.Collections;
using UnityEngine;

namespace MyMetroidVania.Entity.Character.Enemy
{
    public abstract class EnemyBase : MonoBehaviour
    {
        [Header("EnemyBase")]
        [SerializeField] protected AudioSource _audioSource = null;
        [SerializeField] protected Rigidbody2D _rb = null;
        [SerializeField] protected StatusManager _statusManager = null;
        public bool IsDead => _statusManager.IsDead;

        [Header("サウンド（Enemy Base）")]
        [SerializeField, Tooltip("被弾時音源ファイル名")] protected string _takeDamageSoundName;
        protected SoundData _takeDamageSound = null;
        [SerializeField, Tooltip("死亡時音源ファイル名")] protected string _deadSoundName;
        protected SoundData _deadSound = null;

        protected Vector3 _initialPosition; // 初期位置
        private EnemyState _currentState = null; // 現在のステート

        // ダメージを受けたかどうか
        private bool _isStun = false;
        public bool IsStun => _isStun;

        public event Action OnCompletedDeadAnimationEvent;
        public event Action<EnemyBase> OnDestroyed;

        /// <summary>
        /// 初期化処理（初回のみ発火）
        /// </summary>
        public virtual void InitializeOnce()
        {
            _takeDamageSound = AudioManager.Instance.GetSe(_takeDamageSoundName);
            _deadSound = AudioManager.Instance.GetSe(_deadSoundName);
            _initialPosition = transform.position;

            //-イベント-------------------
            // ステータス周り
            _statusManager.OnDamageTaken += OnTakenDamage;
            _statusManager.OnDead += OnDead;
        }

        /// <summary>
        /// 初期化処理
        /// 初回、エリア移動時に発火
        /// </summary>
        public virtual void Initialize()
        {
            // 初期位置に移動
            transform.position = _initialPosition;
            // 生存している場合にのみステータスを初期化
            if (gameObject.activeSelf) _statusManager.InitializeStatus();
        }

        /// <summary>
        /// リスポーン処理
        /// セーブ時、死亡時に発火
        /// </summary>
        public virtual void Respawn()
        {
            // 死亡していた場合にActiveにする
            if (!gameObject.activeSelf) gameObject.SetActive(true);
            // 初期位置に移動
            transform.position = _initialPosition;
            _statusManager.InitializeStatus();
        }

        /// <summary>
        /// 現在のステートのアクションを発火
        /// </summary>
        private void Update()
        {
            _currentState.Tick();
        }

        /*
         * ------------------------------------------------------------------
         * ステート遷移周りで用いるメソッド
         * ------------------------------------------------------------------
         */
        /// <summary>
        /// ステートの遷移
        /// 初期化時または各Stateクラスの処理内で呼び出す。
        /// </summary>
        /// <param name="newState">遷移後のState</param>
        public void ChangeState(EnemyState newState)
        {
            _currentState?.Exit(); // ステートを抜け出す処理
            _currentState = newState;
            newState.Enter(); // ステートに入る処理
        }
        /// <summary>
        /// 検知範囲内にプレイヤーが存在するか返す
        /// </summary>
        public abstract bool IsPlayerDetected();
        /// <summary>
        /// 攻撃射程にプレイヤーが存在するか返す
        /// </summary>
        public abstract bool IsPlayerInRange();
        /// <summary>
        /// スタン状態を解除
        /// </summary>
        public void RecoverStun()
        {
            _isStun = false;
        }


        /*
         * ------------------------------------------------------------------
         * アクションを制御
         * ------------------------------------------------------------------
         */

        // 待機
        public abstract IEnumerator OnIdle();
        public abstract void StopMove();

        // 追跡
        public abstract IEnumerator OnChase();
        public abstract void StopChase();

        // 攻撃
        public abstract IEnumerator OnAttack();


        /*
         * ------------------------------------------------------------------
         * リアクションを制御
         * ------------------------------------------------------------------
         */
        /// <summary>
        /// 被弾時のリアクション
        /// </summary>
        protected virtual void OnTakenDamage()
        {
            AudioManager.Instance.PlayOneShotSe(_takeDamageSound);
            _isStun = true; // スタン状態にする
        }

        /// <summary>
        /// 死亡時のリアクション
        /// アニメーション側でGameObjectをfalseに変更する
        /// </summary>
        protected virtual void OnDead()
        {
            AudioManager.Instance.PlayOneShotSe(_deadSound);
            StopAllCoroutines();
            _currentState?.Exit();
        }

        /// <summary>
        /// 死亡アニメーション完了
        /// </summary>
        protected void OnCompletedDeadAnimation()
        {
            gameObject.SetActive(false);
            OnCompletedDeadAnimationEvent?.Invoke();
        }

        /// <summary>
        /// メインカメラの画角内に存在するかをカメラとエネミーのポジションで計算し
        /// カメラ内に存在するかどうかを返す
        /// ※ メインカメラはプレイヤーに追従している
        /// </summary>
        /// <return>カメラに映っていたら true</return>
        public bool IsVisible()
        {
            Vector3 pos = transform.position;
            Vector3 camPos = Camera.main.transform.position;
            float halfH = Camera.main.orthographicSize; // カメラの縦のサイズの半分
            float halfW = halfH * Camera.main.aspect; // カメラの横のサイズの半分

            // カメラの範囲内かどうか 2fはバッファ
            return pos.x > camPos.x - halfW - 2f && pos.x < camPos.x + halfW + 2f
                && pos.y > camPos.y - halfH - 2f && pos.y < camPos.y + halfH + 2f;
        }

        private void OnDestroy()
        {
            OnDestroyed?.Invoke(this);
        }
    }
}