using MyMetroidVania.Data.ScriptableObjects;
using MyMetroidVania.Entity.Gimmick;
using MyMetroidVania.System;
using MyMetroidVania.Utility;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace MyMetroidVania.Entity.Character.Player
{
    /// <summary>
    /// プレイヤーの全ての挙動を管理する
    /// </summary>
    public class Player : MonoBehaviour
    {
        [SerializeField] private StatusManager _statusManager = null;
        [SerializeField] private PlayerInputHandler _input = null;
        [SerializeField] private PlayerPhysics _physics = null;
        [SerializeField] private PlayerVisualEffect _visualEffect = null;
        [SerializeField] private PlayerAnimation _animation = null;
        private Coroutine _runEffectRoutine = null;

        [SerializeField, Tooltip("アビリティの取得状況を管理")]
        private AbilityManager _abilityManager;

        [Header("ジャンプ")]
        [SerializeField, Tooltip("地面の接地判定")] private BoxCaster _groundChecker;

        [Header("攻撃")]
        [SerializeField, Tooltip("攻撃判定の原点")] private Transform _hitBoxOriginTransform = null;
        [SerializeField, Tooltip("攻撃判定")] private HitBox _hitBox;
        [SerializeField, Tooltip("攻撃CT（秒）")] private float _coolSec = 0.25f;
        private bool _isAttacking = false;

        [Header("フック")]
        [SerializeField, Tooltip("フックの原点")] private Transform _hookOriginTransform = null;
        [SerializeField, Tooltip("フックの箱判定")] private BoxCaster _hookCheckerBox = null;
        [SerializeField, Tooltip("フックが引き寄せる時の早さ")] private float _hookSpeed = 15f;
        [SerializeField, Tooltip("フックが切れる長さ")] private float _hookCancelRange = 0.5f;
        [SerializeField, Tooltip("フックのクールタイム（秒）")] private float _hookCTSeconds = 0.5f;
        private Vector2 _hookPosition;
        private bool _canHook = true;
        private Coroutine _hookCoolDownRoutine = null;
        public event Action<float> OnCoolHook = null; // フックUIで購読

        [Header("インタラクト")]
        [SerializeField, Tooltip("インタラクト検知範囲")] private BoxCaster _interactChecker;

        private enum ActionState
        {
            Idle,             // 待機
            Run,             // 移動
            JumpAnticipation, // ジャンプ準備
            Jump,             // ジャンプ
            Fall,             // 落下
            Hook,             // フック
        }
        private ActionState _currentState = ActionState.Idle;

        void Start()
        {
            Initialize();
            InitializeEvents();
            _visualEffect.Initialize();
        }

        private void Initialize()
        {
            _currentState = ActionState.Run;
        }
        private void InitializeEvents()
        {
            // プレイヤーの操作
            _input.OnJumpStarted += OnJump;
            _input.OnHookStarted += OnHookStarted;
            _input.OnHookCanceled += OnHookCanceled;
            _input.OnAttackStarted += OnAttack;
            _input.OnInteractStarted += OnInteract;

            // ステータス周り
            _statusManager.OnDamageTaken += OnDamageTaken;
            _statusManager.OnDead += OnDead;
        }

        private void DisposeEvents()
        {
            // ステータス周り
            _statusManager.OnDamageTaken -= OnDamageTaken;
            _statusManager.OnDead -= OnDead;
        }

        private void Update()
        {
            // 現在の入力情報を保持
            var dir = _input.InputDirection;

            // プレイヤーの向きをセット
            _visualEffect.SetFlip(dir.x);

            // 移動入力の方向に攻撃判定、フック原点を回転させる。
            if (dir != Vector2.zero)
            {
                var degDir = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                Vector3 newRotate = new Vector3(0, 0, degDir);
                var angle = Quaternion.Euler(newRotate);
                _hitBoxOriginTransform.rotation = angle;
                _hookOriginTransform.rotation = angle;
            }

            switch (_currentState)
            {
                case ActionState.Idle:
                case ActionState.Run:
                    if (!_groundChecker.IsCasted)
                    {
                        _currentState = ActionState.Fall;
                        break;
                    }
                    break;

                case ActionState.JumpAnticipation:
                    if (!_groundChecker.IsCasted)
                    {
                        _currentState = ActionState.Jump;
                        break;
                    }
                    break;

                case ActionState.Jump:
                case ActionState.Fall:
                    if (_groundChecker.IsCasted)
                    {
                        // 着地エフェクトの生成
                        _visualEffect.PlayLandEffect();

                        _currentState = ActionState.Idle;
                        break;
                    }
                    break;

                case ActionState.Hook:
                    break;

                default:
                    break;
            }
        }

        private void FixedUpdate()
        {
            // 移動処理
            if (_currentState != ActionState.Hook)
            {
                _physics.SetMoveVelocity(_input.InputDirection.x);
            }

            switch (_currentState)
            {
                case ActionState.Idle:
                    if (_physics.IsMoving)
                    {
                        // 移動している場合Walkステート
                        _currentState = ActionState.Run;

                        // ランエフェクト
                        if (_runEffectRoutine == null)
                        {
                            _runEffectRoutine = StartCoroutine(PlayRunEffect());
                        }

                        break;
                    }
                    break;

                case ActionState.Run:
                    if (!_physics.IsMoving)
                    {
                        _currentState = ActionState.Idle;
                        break;
                    }

                    // 現在の速さが規定の移動速を超えていた場合に徐々に速さを減らす
                    _physics.ReduceExcessSpeed();
                    break;

                case ActionState.Fall:
                    // 現在の速さが規定の移動速を超えていた場合に徐々に速さを減らす
                    _physics.ReduceExcessSpeed();
                    break;

                case ActionState.JumpAnticipation:
                case ActionState.Jump:
                    if (_physics.Velocity.y < 0)
                    {
                        _currentState = ActionState.Fall;
                        break;
                    }
                    // ジャンプボタンを押している間は上向きの微量な加速をさせ、落下を遅らせる
                    if (_input.IsPressedJumpButton)
                    {
                        _physics.AccelerateJump();
                    }
                    break;

                case ActionState.Hook:
                    // 常にフックの向きに向かって速度を出す
                    var dir = _hookPosition - (Vector2)transform.position;
                    if (dir.magnitude < _hookCancelRange)
                    {
                        _currentState = ActionState.Fall;
                        break;
                    }
                    _physics.SetVelocity(dir.normalized * _hookSpeed);
                    break;

                default:
                    break;
            }

            // アニメーションで参照するパラメータ値を更新
            _animation.UpdateParam(_physics.Velocity, _groundChecker.IsCasted);
        }


        /*
         * ------------------------------------------------------------------
         * ステータスを制御
         * ------------------------------------------------------------------
         */
        public void UnlockAbility(AbilityType type)
        {
            _abilityManager.UnlockAbility(type);
        }

        public void Heal()
        {
            _statusManager.Heal();
        }


        /*
         * ------------------------------------------------------------------
         * リアクションを制御
         * ------------------------------------------------------------------
         */
        /// <summary>
        /// 被弾時のリアクション
        /// </summary>
        private void OnDamageTaken()
        {
            // ノックバックなど入れるかも
        }

        /// <summary>
        /// 死亡時のリアクション
        /// </summary>
        private void OnDead()
        {
            _statusManager.InitializeStatus();
            WorldManager.Instance.RespawnPlayer();
        }


        /*
         * ------------------------------------------------------------------
         * 移動を制御
         * ------------------------------------------------------------------
         */
        /// <summary>
        /// 走るエフェクトの再生ルーチン
        /// ステートを見て自分で処理を終了する
        /// </summary>
        private IEnumerator PlayRunEffect()
        {
            while (true)
            {
                // ステートがRunでない場合、ルーチンを抜ける
                if (_currentState != ActionState.Run)
                {
                    _runEffectRoutine = null;
                    yield break;
                }

                // 走った際の砂埃エフェクト
                yield return _visualEffect.PlayRunEffect(_input.InputDirection.x);
            }
        }

        /*
         * ------------------------------------------------------------------
         * 攻撃挙動を制御
         * ------------------------------------------------------------------
         */

        private void OnAttack()
        {
            // 後々アニメーションで制御することになりそう
            if (_isAttacking) return;
            Debug.Log("攻撃！");
            StartCoroutine(Attack());
        }

        private IEnumerator Attack()
        {
            _isAttacking = true;
            _hitBox.SetEnableCollider();

            yield return new WaitForSeconds(_coolSec);

            _hitBox.SetDisableCollider();
            _isAttacking = false;
        }


        /*
         * ------------------------------------------------------------------
         * ジャンプ挙動を制御
         * ------------------------------------------------------------------
         */
        /// <summary>
        /// ジャンプボタンを押した瞬間に発火
        /// </summary>
        private void OnJump()
        {
            if (!_groundChecker.IsCasted) return;

            _currentState = ActionState.JumpAnticipation;
            Jump();
        }

        /// <summary>
        /// ジャンプする
        /// </summary>
        private void Jump()
        {
            _physics.Jump();
            _visualEffect.PlayJumpEffect();

            _animation.TriggerJump();
        }

        /*
         * ------------------------------------------------------------------
         * フック機能を制御
         * ------------------------------------------------------------------
         */
        /// <summary>
        /// フック処理
        /// ボタン長押し中は引っ張られる
        /// </summary>
        private void OnHookStarted()
        {
            // フックが使用不可であるかどうか
            if (!_abilityManager.HasAbility(AbilityType.Hook) || !_canHook) return;

            // フックをかけるポイントがあるかどうか
            // キャスト切ったほうがいいかも
            if (!_hookCheckerBox.IsCasted) return;

            RaycastHit2D hit = default;
            if (_hookCheckerBox != null)
            {
                hit = _hookCheckerBox.GetBoxCast();
            }

            if (hit.collider != null)
            {
                _currentState = ActionState.Hook;
                _hookPosition = hit.point;
                _visualEffect.PlayHookEffect();
            }
        }

        /// <summary>
        /// フック処理
        /// ボタンを離したときはステートを戻す
        /// </summary>
        private void OnHookCanceled()
        {
            // フックが使用不可であるかどうか
            if (!_abilityManager.HasAbility(AbilityType.Hook)) return;

            _currentState = ActionState.Run;
            if (_hookCoolDownRoutine == null)
            {
                // フックのクールダウン開始
                _hookCoolDownRoutine = StartCoroutine(WaitHookCooldown());
            }
        }

        /// <summary>
        /// フックのクールダウン
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitHookCooldown()
        {
            _canHook = false;

            float timer = 0f;
            while (timer < _hookCTSeconds)
            {
                yield return null;
                timer += Time.deltaTime;
                OnCoolHook?.Invoke(timer / _hookCTSeconds); // クールタイムの進捗
            }

            _canHook = true;
            OnCoolHook?.Invoke(0); // 非表示
            _hookCoolDownRoutine = null;
        }


        /*
         * ------------------------------------------------------------------
         * インタラクト機能を制御
         * ------------------------------------------------------------------
         */
        /// <summary>
        /// インタラクト処理
        /// </summary>
        private void OnInteract()
        {
            if (!_interactChecker.TryGetClosestCollider(out var obj)) return;

            if (obj.TryGetComponent<IInteractable>(out var interactable))
            {
                interactable.Interact(this);
            }
        }

        private void OnDestroy()
        {
            DisposeEvents();
        }
    }
}