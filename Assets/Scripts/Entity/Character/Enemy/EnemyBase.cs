using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("EnemyBase")]
    [SerializeField] protected AudioSource _audioSource = null;
    [SerializeField] protected Rigidbody2D _rb = null;
    [SerializeField] protected StatusManager _statusManager = null;
    [SerializeField, Tooltip("x軸の移動の速さ")] protected float _moveSpeedX = 5f;
    [SerializeField, Tooltip("地面の接地判定")] protected BoxCaster _groundChecker = null;

    [Header("サウンド（Enemy Base）")]
    [SerializeField, Tooltip("被弾時音源ファイル名")] protected string _takeDamageSoundName;
    protected SoundData _takeDamageSound = null;
    [SerializeField, Tooltip("死亡時音源ファイル名")] protected string _deadSoundName;
    protected SoundData _deadSound = null;

    // 初期位置
    private Vector3 _initialPosition;

    protected enum ActionState
    {
        Walk,
        JumpAnticipation,
        Jump,
        Hook,
    }
    protected ActionState _currentState = ActionState.Walk;

    /// <summary>
    /// 初期化処理（初回のみ発火）
    /// </summary>
    public virtual void InitializeOnce()
    {
        _takeDamageSound = AudioManager.Instance.GetSe(_takeDamageSoundName.GetHashCode());
        _deadSound = AudioManager.Instance.GetSe(_deadSoundName.GetHashCode());
        _initialPosition = transform.position;

        //-イベント-------------------
        // ステータス周り
        _statusManager.OnDamaged += Damaged;
        _statusManager.OnDead += Dead;
    }

    /// <summary>
    /// 初期化処理（初回、セーブ時、死亡時に発火）
    /// </summary>
    public virtual void Initialize()
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        transform.position = _initialPosition;
        _statusManager.InitializeStatus();
    }

    /*
     * ------------------------------------------------------------------
     * リアクションを制御
     * ------------------------------------------------------------------
     */
    /// <summary>
    /// 被弾時のリアクション
    /// </summary>
    protected virtual void Damaged()
    {
        AudioManager.Instance.PlayOneShotSe(_takeDamageSound);
    }

    /// <summary>
    /// 死亡時のリアクション
    /// </summary>
    protected virtual void Dead()
    {
        AudioManager.Instance.PlayOneShotSe(_deadSound);
        StopAllCoroutines();

        gameObject.SetActive(false);
    }
}
