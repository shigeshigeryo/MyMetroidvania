using UnityEngine;

/// <summary>
/// 世界の状態を管理するスクリプト
/// </summary>
public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance { get; private set; }

    private const string WORLD_STATE_DATA_PATH = "WorldData/WorldStateData";
    // 世界共通の状態データ
    private WorldStateData _worldStateData;
    public WorldStateData WorldStateData => _worldStateData;

    // 現在アクティブになっているエリア
    public AreaManager CurrentAreaManager;

    [SerializeField] private Player _player;
    // リスポーン地点は全てセーブポイントでない可能性があるため別で保持しておく
    private Vector3 _respawnPosition;
    private SavePoint _currentSavePoint = null;

#if UNITY_EDITOR
    [SerializeField] private bool _isDebug = false;
#endif

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // 世界全ての情報
        if(!JsonHandler.TryLoadJsonFile<WorldStateData>(WORLD_STATE_DATA_PATH, out _worldStateData))
        {
            _worldStateData = JsonHandler.LoadResourcesJsonFile<WorldStateData>(WORLD_STATE_DATA_PATH);
            SaveWorldStateData();
        }
        Debug.Assert(_worldStateData != default);
    }

    private void Start()
    {
        CurrentAreaManager = AreaManager.AreaManagerList["Area_001"];
        CurrentAreaManager.gameObject.SetActive(true);

        // TODO:セーブからロードする方式に直す
        _respawnPosition = _player.transform.position;
    }

    /// <summary>
    /// エリア間移動の通知
    /// </summary>
    /// <param name="areaId">移動先エリア</param>
    /// <param name="spawnPosition">移動先のスポーン地点</param>
    public void ChangeArea(string areaId, Vector3 spawnPosition)
    {
        // 移動前エリアをInactive
        CurrentAreaManager.gameObject.SetActive(false);

        CurrentAreaManager = AreaManager.AreaManagerList[areaId];
        // 移動後のエリアをActive
        CurrentAreaManager.gameObject.SetActive(true);
        // 移動先にスポーンさせる
        _player.transform.position = spawnPosition;
    }

    /// <summary>
    /// プレイヤーをリスポーンする
    /// </summary>
    public void RespawnPlayer()
    {
        _player.transform.position = _respawnPosition;
    }

    /// <summary>
    /// 最近でアクセスしたセーブポイントの更新
    /// </summary>
    /// <param name="newSavePoint"></param>
    public void SetCurrentSavePoint(SavePoint newSavePoint)
    {
        if (_currentSavePoint != null)
        {
            // 最近でアクセスしたセーブポイントを更新するため
            // 現在最新状態として保存されているStateを更新
            _currentSavePoint.ChangeState(SavePoint.State.Accessed);
        }
        _currentSavePoint = newSavePoint;

        _respawnPosition = newSavePoint.transform.position;
    }

    private void SaveWorldStateData()
    {
#if UNITY_EDITOR
        if (_isDebug) return;
#endif
        JsonHandler.WriteJsonFile(WORLD_STATE_DATA_PATH, _worldStateData);
    }

    private void OnDestroy()
    {
        SaveWorldStateData();
    }
}
