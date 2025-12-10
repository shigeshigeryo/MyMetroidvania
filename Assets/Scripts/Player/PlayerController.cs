using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤーの全ての挙動を管理する
/// </summary>
public class PlayerController : MonoBehaviour
{
    private PlayerInputActions _inputActions;　// PlayerInputのイベント
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField, Tooltip("x軸の移動の速さ")] private float _moveSpeedX = 5f;
    [SerializeField, Tooltip("ジャンプの初速")] private float _jumpSpeed = 8f;
    [SerializeField, Tooltip("ジャンプボタン押下時にかかる+yの加速度")] private float _jumpAccel = 10f;
    [SerializeField, Tooltip("地面の接地判定")] private BoxCaster _groundChecker;

    private Vector2 _inputDirection = Vector2.zero;
    private bool _isPushedJumpButton = false;

    private enum ActionState
    {
        Walk,
        JumpAnticipation,
        Jump,
        Attack,
    }
    private ActionState _currentState = ActionState.Walk;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
    }

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _currentState = ActionState.Walk;
        _isPushedJumpButton = false;
        InitializeInputAction();
    }

    private void Update()
    {
        _inputDirection = _inputActions.Player.Move.ReadValue<Vector2>();

    }

    private void FixedUpdate()
    {
        _rb.linearVelocityX = _moveSpeedX * _inputDirection.x;

        switch (_currentState)
        {
            case ActionState.Walk:
                break;
            case ActionState.JumpAnticipation:
                if (!_groundChecker.IsCasted)
                {
                    _currentState = ActionState.Jump;
                    break;
                }

                // ジャンプボタンを押している間は上向きの微量な加速をさせ、落下を遅らせる
                if (_isPushedJumpButton)
                {
                    AccelerateJump();
                }
                break;

            case ActionState.Jump:
                if (_groundChecker.IsCasted)
                {
                    _currentState = ActionState.Walk;
                    break;
                }

                // ジャンプボタンを押している間は上向きの微量な加速をさせ、落下を遅らせる
                if (_isPushedJumpButton)
                {
                    AccelerateJump();
                }
                break;

            case ActionState.Attack:
                break;
            default:
                break;
        }
        Debug.Log(_currentState);
    }

    private void InitializeInputAction()
    {
        _inputActions.Enable();
        _inputActions.Player.Jump.started += OnJump;
        _inputActions.Player.Jump.canceled += OnJump;
    }

    private void DisposeInputAction()
    {
        _inputActions.Player.Jump.started -= OnJump;
        _inputActions.Player.Jump.canceled -= OnJump;
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
        var newVelocity = _rb.linearVelocity;
        newVelocity.y = _jumpSpeed;
        _rb.linearVelocity = newVelocity; ;
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

    private void OnDestroy()
    {
        DisposeInputAction();
    }
}
