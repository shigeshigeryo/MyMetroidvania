using UnityEngine;

public class Door : GimmickBase, IInteractable
{
    [System.Serializable]
    public enum State
    {
        Close, // 閉鎖
        Open, // 開放
        Lock // 施錠
    }
    private State _currentState;
    [SerializeField, Tooltip("見た目")] private SpriteRenderer _visual;
    [SerializeField, Tooltip("コライダー")] private Collider2D _collider;
    [SerializeField, Tooltip("ドアを開く音源ファイル名")] private string _openSoundName = "SE_DoorOpen";
    SoundData _openSoundData;

    private void Start()
    {
        InitializeState();
        _openSoundData = AudioManager.Instance.GetSe(_openSoundName.GetHashCode());
    }

    /// <summary>
    /// ドアの状態を初期化
    /// </summary>
    protected override void InitializeState()
    {
        if (AreaManager.Instance.AreaStateData.TryGetTargetState(Id, out var stateData))
        {
            _currentState = (State)stateData.State;
        }

        switch (_currentState)
        {
            case State.Open:
                Open();
                break;

            case State.Lock:
                // 鍵付き特有の状態
                break;
            
            // 初期状態のため処理は不要
            case State.Close:
            default:
                break;
        }
    }

    public void Interact()
    {
        AudioManager.Instance.PlayOneShotSe(_openSoundData);
        Open();
    }

    /// <summary>
    /// 開放状態
    /// </summary>
    private void Open()
    {
        _visual.enabled = false;
        _collider.enabled = false;
        _currentState = State.Open;
    }
}
