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
    private string _lastRespawnAreaId;

    // 現在アクティブになっているエリア
    private AreaManager _currentAreaManager;

    [SerializeField] private Player _player;
    // リスポーン地点は全てセーブポイントでない可能性があるため別で保持しておく
    private Vector3 _respawnPosition;
    private SavePoint _currentSavePoint = null;
    private bool _isInitializeSpawn = false;

#if UNITY_EDITOR
    [SerializeField] private bool _isDebug = false;
#endif

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 世界全ての情報
        if(!JsonHandler.TryLoadJsonFile<WorldStateData>(WORLD_STATE_DATA_PATH, out _worldStateData))
        {
            _worldStateData = JsonHandler.LoadResourcesJsonFile<WorldStateData>(WORLD_STATE_DATA_PATH);
            _lastRespawnAreaId = _worldStateData.LastRespawnAreaId;
            SaveWorldStateData();
        }
        Debug.Assert(_worldStateData != default);
    }

    private void Start()
    {
        MapManager.Instance.Initialize(_worldStateData.VisitedAreaIdList);

        _lastRespawnAreaId = _worldStateData.LastRespawnAreaId;
        _currentAreaManager = AreaManager.AreaManagerList[_lastRespawnAreaId];
        _currentAreaManager.gameObject.SetActive(true);
    }

    /// <summary>
    /// エリア間移動の通知
    /// </summary>
    /// <param name="areaId">移動先エリア</param>
    /// <param name="spawnPosition">移動先のスポーン地点</param>
    public void ChangeArea(string areaId, Vector3 spawnPosition)
    {
        // 移動前エリアをInactive
        _currentAreaManager.gameObject.SetActive(false);

        _currentAreaManager = AreaManager.AreaManagerList[areaId];

        // 訪れたことがないエリアだった場合に追加し、エリアマップを表示する
        if (!_worldStateData.VisitedAreaIdList.Contains(areaId))
        {
            _worldStateData.VisitedAreaIdList.Add(areaId);
            MapManager.Instance.SetVisitedArea(areaId);
        }
        
        // 移動後のエリアをActive
        _currentAreaManager.gameObject.SetActive(true);
        // 移動先にスポーンさせる
        _player.transform.position = spawnPosition;
    }

    /// <summary>
    /// プレイヤーをリスポーンする
    /// </summary>
    public void RespawnPlayer()
    {
        // 現在いるエリアとセーブポイントのエリアが異なる場合は、
        // エリアを切り替えてからスポーンさせる必要がある
        if(_currentAreaManager.AreaId != _lastRespawnAreaId)
        {
            ChangeArea(_lastRespawnAreaId, _respawnPosition);
        }
        else
        {
            _player.transform.position = _respawnPosition;
        }

        foreach (var am in AreaManager.AreaManagerList.Values)
        {
            am.InitializeAreaState();
        }
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
        // アクセスされるセーブポイントがあるエリアは必ず現在のエリアになる
        _lastRespawnAreaId = _currentAreaManager.AreaId;

        // 初回はプレイヤーをスポーンさせる
        if (!_isInitializeSpawn)
        {
            _isInitializeSpawn = true;
            RespawnPlayer();
        }
    }

    private void SaveWorldStateData()
    {
#if UNITY_EDITOR
        if (_isDebug) return;
#endif
        _worldStateData.SetLastRespawnAreaId(_lastRespawnAreaId);
        JsonHandler.WriteJsonFile(WORLD_STATE_DATA_PATH, _worldStateData);
    }

    private void OnDestroy()
    {
        SaveWorldStateData();
    }
}
