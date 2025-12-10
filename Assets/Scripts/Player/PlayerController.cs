using UnityEngine;

/// <summary>
/// プレイヤーの全ての挙動を管理する
/// </summary>
public class PlayerController : MonoBehaviour
{
    private PlayerInputActions _inputActions;　// PlayerInputのイベント
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField, Tooltip("x軸の移動の速さ")] private float _moveSpeedX = 5f;

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
    }
}
