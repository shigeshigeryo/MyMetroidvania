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
    [SerializeField, Tooltip("ジャンプの初速")] private float _jumpSpeed = 5f;
    [SerializeField, Tooltip("地面の接地判定")] private BoxCaster _groundChecker;

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
        InitializeInputAction();
    }

    private void Update()
    {
        var direction = _inputActions.Player.Move.ReadValue<Vector2>();
        _rb.linearVelocityX = _moveSpeedX * direction.x;
    }

    private void InitializeInputAction()
    {
        _inputActions.Enable();
        _inputActions.Player.Jump.started += OnJump;
    }

    /// <summary>
    /// ジャンプボタンを押した瞬間に発火
    /// </summary>
    private void OnJump(InputAction.CallbackContext _)
    {
        Debug.Log("triggered");
        if (_groundChecker.IsCasted)
        {
            Debug.Log(true);
            var newVelocity = _rb.linearVelocity;
            newVelocity.y = _jumpSpeed;
            _rb.linearVelocity = newVelocity;
        }
    }
}
