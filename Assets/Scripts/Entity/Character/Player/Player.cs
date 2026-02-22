using MyMetroidVania.Entity.Effect;
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
        [SerializeField, Tooltip("足元のコライダー")] private Collider2D _footCollider;
        [SerializeField, Tooltip("通りぬけ可能なレイヤーを指定")] private LayerMask _canPathThroughLayer = 0;

        [Header("攻撃")]
        [SerializeField, Tooltip("攻撃判定の原点")] private Transform _hitBoxOriginTransform = null;
        [SerializeField, Tooltip("攻撃CT（秒）")] private float _coolSec = 0.25f;
        private bool _isAttacking = false;
        [SerializeField, Tooltip("手裏剣")] Shuriken _shurikenPrefab = null;
        private IObjectPool<Shuriken> _shurikenPool = null;

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
            _currentState = ActionState.Idle;

            // 手裏剣プール
            _shurikenPool = new ObjectPool<Shuriken>(
                createFunc: () =>
                {
                    Shuriken shuriken = Instantiate(_shurikenPrefab);
                    shuriken.SetPool(_shurikenPool);
                    return shuriken;
                },
                actionOnGet: (shuriken) =>
                {
                    shuriken.gameObject.SetActive(true);
                },
                actionOnRelease: (shuriken) =>
                {
                    shuriken.gameObject.SetActive(false);
                },
                actionOnDestroy: (shuriken) =>
                {
                    Destroy(shuriken.gameObject);
                },
                defaultCapacity: 3, // 準備数（仮）
                maxSize: 5 // 最大数（仮）
            );
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
            _statusManager.OnDamageTaken += _input.VibrateController;
            _statusManager.OnDead += _input.VibrateController;
            _statusManager.OnDead += OnDead;
        }

        private void DisposeEvents()
        {
            // ステータス周り
            _statusManager.OnDamageTaken -= _input.VibrateController;
            _statusManager.OnDead -= _input.VibrateController;
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
                        _visualEffect.StopRunSound();
                        break;
                    }
                    break;

                case ActionState.JumpAnticipation:
                    if (!_groundChecker.IsCasted)
                    {
                        _currentState = ActionState.Jump;
                        _visualEffect.StopRunSound();
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
                        _visualEffect.PlayRunSound();

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
                        _visualEffect.StopRunSound();
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
                        _visualEffect.StopHookEffect();
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

        /// <summary>
        /// 攻撃の発火
        /// </summary>
        private void OnAttack()
        {
            if (_isAttacking) return;
            StartCoroutine(Attack());
        }

        /// <summary>
        /// 攻撃の処理とクールタイムを管理
        /// </summary>
        private IEnumerator Attack()
        {
            _isAttacking = true;
            var shuriken = _shurikenPool.Get();
            shuriken.Initialize(_hitBoxOriginTransform.position,
                    _hitBoxOriginTransform.rotation,
                    _statusManager.GetAttackPower());
            _visualEffect.PlayShurikenSound();

            yield return new WaitForSeconds(_coolSec);

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

            if (_input.InputDirection.y < -0.5f)
            {
                // 下入力がある場合は通りぬけ
                StartCoroutine(PathThroughPlatform());
            }
            else
            {
                // 下入力がない場合は通常のジャンプ
                _currentState = ActionState.JumpAnticipation;
                Jump();
            }
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

        /// <summary>
        /// 下＋ジャンプ入力でプラットフォームを通りぬける
        /// 足元のコライダーとプラットフォームのレイヤーを一時的に判定しないようにする
        /// 足元以外のコライダーにはヒットしないように事前にExcluderLayersに設定済み
        /// </summary>
        private IEnumerator PathThroughPlatform()
        {
            _footCollider.excludeLayers = _canPathThroughLayer;
            yield return new WaitForSeconds(0.2f);
            _footCollider.excludeLayers = 0; // Nothing
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

            RaycastHit2D hit = default;
            if (_hookCheckerBox != null)
            {
                hit = _hookCheckerBox.GetBoxCast();
            }

            if (hit.collider != null)
            {
                _currentState = ActionState.Hook;
                _hookPosition = hit.point;
                _visualEffect.PlayHookEffect(_hookPosition);
            }
        }

        /// <summary>
        /// フック処理
        /// ボタンを離したときはステートを戻す
        /// </summary>
        private void OnHookCanceled()
        {
            // フック状態であるかどうか
            if (_currentState != ActionState.Hook) return;

            _currentState = ActionState.Run;
            _visualEffect.StopHookEffect();
            if (_hookCoolDownRoutine == null)
            {
                // フックのクールダウン開始
                _hookCoolDownRoutine = StartCoroutine(WaitHookCooldown());
            }
        }

        /// <summary>
        /// フックのクールダウン
        /// クールタイム && クールタイム後に着地している と再使用可能
        /// </summary>
        private IEnumerator WaitHookCooldown()
        {
            _canHook = false;

            float timer = 0f;
            while (true)
            {
                yield return null;
                timer += Time.deltaTime;
                if (timer >= _hookCTSeconds && _groundChecker.IsCasted) break;
            }

            _visualEffect.PlayHookCTCompleteEffect();
            _hookCoolDownRoutine = null;
            _canHook = true;
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