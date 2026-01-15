using MyMetroidVania.Data.ScriptableObjects;
using MyMetroidVania.Entity.Gimmick;
using MyMetroidVania.System;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using MyMetroidVania.Utility;

namespace MyMetroidVania.Entity.Character.Player
{
    /// <summary>
    /// プレイヤーの全ての挙動を管理する
    /// </summary>
    public class Player : MonoBehaviour
    {
        private PlayerInputActions _inputActions = null; // PlayerInputのイベント
        [SerializeField] private AudioSource _audioSource = null;
        [SerializeField] private Rigidbody2D _rb = null;
        [SerializeField] private StatusManager _statusManager = null;
        [SerializeField, Tooltip("x軸の移動の速さ")] private float _moveSpeedX = 5f;
        [SerializeField, Tooltip("Walk中に移動速度を超えたときに抵抗としてかかる毎秒の速度")]
        private float _deceleration = 10f;
        [SerializeField, Tooltip("ジャンプの初速")] private float _jumpSpeed = 8f;
        [SerializeField, Tooltip("ジャンプボタン押下時にかかる+yの加速度")] private float _jumpAccel = 10f;
        [SerializeField, Tooltip("地面の接地判定")] private BoxCaster _groundChecker;
        [SerializeField, Tooltip("アビリティの取得状況を管理")]
        private AbilityManager _abilityManager;

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

        private Vector2 _inputDirection = Vector2.zero;
        private bool _isPushedJumpButton = false;

        [Header("サウンド")]
        [SerializeField, Tooltip("ジャンプ音源ファイル名")] private string _jumpSoundName = "SE_PlayerJump";
        private SoundData _jumpSound = null;
        [SerializeField, Tooltip("フック音源ファイル名")] private string _hookSoundName = "SE_PlayerHook";
        private SoundData _hookSound = null;
        [SerializeField, Tooltip("被弾時音源ファイル名")] private string _takeDamageSoundName = "SE_PlayerTakeDamage";
        private SoundData _takeDamageSound = null;
        [SerializeField, Tooltip("死亡時音源ファイル名")] private string _deadSoundName = "SE_PlayerDead";
        private SoundData _deadSound = null;

        private enum ActionState
        {
            Walk,
            JumpAnticipation,
            Jump,
            Hook,
        }
        private ActionState _currentState = ActionState.Walk;

        private void Awake()
        {
            _inputActions = new PlayerInputActions();
        }

        void Start()
        {
            Initialize();
            InitializeEvents();
        }

        private void Initialize()
        {
            _currentState = ActionState.Walk;
            _isPushedJumpButton = false;

            _jumpSound = AudioManager.Instance.GetSe(_jumpSoundName.GetHashCode());
            _hookSound = AudioManager.Instance.GetSe(_hookSoundName.GetHashCode());
            _takeDamageSound = AudioManager.Instance.GetSe(_takeDamageSoundName.GetHashCode());
            _deadSound = AudioManager.Instance.GetSe(_deadSoundName.GetHashCode());
        }

        private void Update()
        {
            _inputDirection = _inputActions.Player.Move.ReadValue<Vector2>();

            // 移動入力の方向に攻撃判定、フック原点を回転させる。
            if (_inputDirection != Vector2.zero)
            {
                var degDir = Mathf.Atan2(_inputDirection.y, _inputDirection.x) * Mathf.Rad2Deg;
                Vector3 newRotate = new Vector3(0, 0, degDir);
                var angle = Quaternion.Euler(newRotate);
                _hitBoxOriginTransform.rotation = angle;
                _hookOriginTransform.rotation = angle;
            }

            switch (_currentState)
            {
                case ActionState.Walk:
                case ActionState.JumpAnticipation:
                    if (!_groundChecker.IsCasted)
                    {
                        _currentState = ActionState.Jump;
                        break;
                    }
                    break;

                case ActionState.Jump:
                    if (_groundChecker.IsCasted)
                    {
                        _currentState = ActionState.Walk;
                        break;
                    }
                    break;

                case ActionState.Hook:
                    break;
            }
        }

        private void FixedUpdate()
        {
            if (_currentState != ActionState.Hook)
            {
                // フック後で速度が出ている場合はそのままの速度を保たせる
                // TODO：フック後に移動していないと不自然に止まるので、直すかどうか検討
                if (Mathf.Abs(_moveSpeedX * _inputDirection.x) > Mathf.Abs(_rb.linearVelocityX) // 入力値が現在の早さを上回るか
                    || Mathf.Sign(_inputDirection.x) != Mathf.Sign(_rb.linearVelocityX) // 速度方向は一致していないか
                    || Mathf.Abs(_inputDirection.x) < 0.01f) // x軸の入力が0付近か
                {
                    _rb.linearVelocityX = _moveSpeedX * _inputDirection.x;
                }
            }

            switch (_currentState)
            {
                case ActionState.Walk:
                    // 現在の速さが規定の移動速を超えていた場合に徐々に速さを減らす
                    if (Mathf.Abs(_rb.linearVelocityX) > _moveSpeedX)
                    {
                        float flg = _rb.linearVelocityX >= 0 ? -1 : 1;
                        _rb.linearVelocityX += flg * _deceleration * Time.fixedDeltaTime;
                    }
                    break;
                case ActionState.JumpAnticipation:
                case ActionState.Jump:
                    // ジャンプボタンを押している間は上向きの微量な加速をさせ、落下を遅らせる
                    if (_isPushedJumpButton)
                    {
                        AccelerateJump();
                    }
                    break;

                case ActionState.Hook:
                    // 常にフックの向きに向かって速度を出す
                    var dir = _hookPosition - (Vector2)transform.position;
                    if (dir.magnitude < _hookCancelRange)
                    {
                        _currentState = ActionState.Walk;
                        break;
                    }
                    _rb.linearVelocity = dir.normalized * _hookSpeed;
                    break;

                default:
                    break;
            }
        }

        private void InitializeEvents()
        {
            // プレイヤーの操作
            _inputActions.Enable();
            _inputActions.Player.Jump.started += OnJump;
            _inputActions.Player.Jump.canceled += OnJump;
            _inputActions.Player.Hook.performed += OnHook;
            _inputActions.Player.Hook.canceled += OnHook;
            _inputActions.Player.Attack.started += OnAttack;
            _inputActions.Player.Interact.started += OnInteract;

            // ステータス周り
            _statusManager.OnDamageTaken += OnDamageTaken;
            _statusManager.OnDead += OnDead;
        }

        private void DisposeEvents()
        {
            // プレイヤーの操作
            _inputActions.Player.Jump.started -= OnJump;
            _inputActions.Player.Jump.canceled -= OnJump;
            _inputActions.Player.Hook.performed -= OnHook;
            _inputActions.Player.Hook.canceled -= OnHook;
            _inputActions.Player.Attack.started -= OnAttack;
            _inputActions.Player.Interact.started -= OnInteract;

            // ステータス周り
            _statusManager.OnDamageTaken -= OnDamageTaken;
            _statusManager.OnDead -= OnDead;
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
            AudioManager.Instance.PlayOneShotSe(_takeDamageSound);
        }

        /// <summary>
        /// 死亡時のリアクション
        /// </summary>
        private void OnDead()
        {
            _statusManager.InitializeStatus();
            AudioManager.Instance.PlayOneShotSe(_deadSound);
            WorldManager.Instance.RespawnPlayer();
        }


        /*
         * ------------------------------------------------------------------
         * 攻撃挙動を制御
         * ------------------------------------------------------------------
         */

        private void OnAttack(InputAction.CallbackContext _)
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
        private void OnJump(InputAction.CallbackContext context)
        {
            // ジャンプボタンを押した時
            if (context.started)
            {
                if (_groundChecker.IsCasted)
                {
                    _currentState = ActionState.JumpAnticipation;
                    _isPushedJumpButton = true;
                    Jump();
                }
            }
            // ジャンプボタンを離した時
            else if (context.canceled)
            {
                _isPushedJumpButton = false;
            }
        }

        /// <summary>
        /// ジャンプする
        /// </summary>
        private void Jump()
        {
            _audioSource.PlayOneShot(_jumpSound.Clip, _jumpSound.Volume);

            var newVelocity = _rb.linearVelocity;
            newVelocity.y = _jumpSpeed;
            _rb.linearVelocity = newVelocity;
        }

        /// <summary>
        /// ジャンプ中にジャンプボタンを押している時にかかる上向き正の加速
        /// </summary>
        private void AccelerateJump()
        {
            var tmpVelocity = _rb.linearVelocity;
            // 落下し始めたタイミングから加速を切る
            if (tmpVelocity.y < 0) return;

            tmpVelocity.y += _jumpAccel * Time.fixedDeltaTime;
            _rb.linearVelocity = tmpVelocity;
        }


        /*
         * ------------------------------------------------------------------
         * フック機能を制御
         * ------------------------------------------------------------------
         */
        /// <summary>
        /// フック処理
        /// ボタン長押し中は引っ張られる
        /// ボタンを離したときはステートを戻す
        /// </summary>
        /// <param name="context"></param>
        private void OnHook(InputAction.CallbackContext context)
        {
            if (!_abilityManager.HasAbility(AbilityType.Hook) || !_canHook) return;

            if (context.performed)
            {
                if (!_hookCheckerBox.IsCasted) return;

                RaycastHit2D hit = default;
                if (_hookCheckerBox != null)
                {
                    hit = _hookCheckerBox.GetBoxCast();
                }

                if (hit.collider != null)
                {
                    _currentState = ActionState.Hook;
                    _audioSource.PlayOneShot(_hookSound.Clip, _hookSound.Volume);
                    _hookPosition = hit.point;
                }
            }
            else if (context.canceled)
            {
                _currentState = ActionState.Walk;
                if (_hookCoolDownRoutine == null)
                {
                    _hookCoolDownRoutine = StartCoroutine(WaitHookCooldown());
                }
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
        private void OnInteract(InputAction.CallbackContext _)
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