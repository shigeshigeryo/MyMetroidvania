using System.Collections;
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

    private Vector3 _initialPosition; // 初期位置
    private EnemyState _currentState = null; // 現在のステート

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
        _statusManager.OnDamageTaken += OnDamageTaken;
        _statusManager.OnDead += OnDead;
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


    /*
     * ------------------------------------------------------------------
     * アクションを制御
     * ------------------------------------------------------------------
     */

    // 待機
    public abstract IEnumerator OnMove();
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
    protected virtual void OnDamageTaken()
    {
        AudioManager.Instance.PlayOneShotSe(_takeDamageSound);
    }

    /// <summary>
    /// 死亡時のリアクション
    /// </summary>
    protected virtual void OnDead()
    {
        AudioManager.Instance.PlayOneShotSe(_deadSound);
        StopAllCoroutines();

        gameObject.SetActive(false);
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
        return (pos.x > camPos.x - halfW - 2f && pos.x < camPos.x + halfW + 2f
            && pos.y > camPos.y - halfH - 2f && pos.y < camPos.y + halfH + 2f);
    }
}
