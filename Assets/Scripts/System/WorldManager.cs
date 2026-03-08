using MyMetroidVania.System.Map;
using UnityEngine;
using MyMetroidVania.Entity.Gimmick;
using MyMetroidVania.Entity.Character.Player;
using MyMetroidVania.Data;

namespace MyMetroidVania.System
{
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
        private bool _isInitializeSpawn = true;

#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField, Tooltip("デバッグ用フラグ　エリアマネージャにも影響あり")] public bool IsDebug = false;
        [SerializeField, Tooltip("デバッグ用初期スポーンエリア名")] private string _debugSpawnPointAreaId = "";
        [SerializeField, Tooltip("デバッグ用初期スポーン地点")] private Transform _debugSpawnPoint = null;
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
                return;
            }

            // 世界全ての情報
            if (!JsonHandler.TryLoadJsonFile(WORLD_STATE_DATA_PATH, out _worldStateData))
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
            // エリアの初期化
            _currentAreaManager.InitializeAreaState();
            // 移動先にスポーンさせる
            _player.transform.position = spawnPosition;

            _player.CancelHook();
        }

        /// <summary>
        /// セーブポイントアクセス時、リスポーン時に発火する全てのエリアの初期化処理
        /// </summary>
        private void InitializeAllAreaRespawn()
        {
            foreach (var am in AreaManager.AreaManagerList.Values)
            {
                am.InitializeAreaStateRespawn();
            }
        }

        /// <summary>
        /// プレイヤーをリスポーンする
        /// </summary>
        public void RespawnPlayer()
        {
            // 現在いるエリアとセーブポイントのエリアが異なる場合は、
            // エリアを切り替えてからスポーンさせる必要がある
            if (_currentAreaManager.AreaId != _lastRespawnAreaId || _isInitializeSpawn)
            {
                GameManager.Instance.ChangeArea(_lastRespawnAreaId, _respawnPosition);
            }
            else
            {
                _player.transform.position = _respawnPosition;
            }

            InitializeAllAreaRespawn();
        }

        /// <summary>
        /// 最近でアクセスしたセーブポイントの更新
        /// </summary>
        /// <param name="newSavePoint"></param>
        public void SetCurrentSavePoint(SavePoint newSavePoint)
        {
            if (_currentSavePoint != null && newSavePoint != _currentSavePoint)
            {
                // 最近でアクセスしたセーブポイントを更新するため
                // 現在最新状態として保存されているStateを更新
                _currentSavePoint.ChangeState(SavePoint.State.Accessed);
            }
            _currentSavePoint = newSavePoint;
            _respawnPosition = newSavePoint.transform.position;
            // アクセスされるセーブポイントがあるエリアは必ず現在のエリアになる
            _lastRespawnAreaId = _currentAreaManager.AreaId;

#if UNITY_EDITOR
            // デバッグ用 指定の位置に初期スポーンさせる
            if (IsDebug && _isInitializeSpawn && _debugSpawnPointAreaId != "" && _debugSpawnPoint != null)
            {
                _lastRespawnAreaId = _debugSpawnPointAreaId;
                _respawnPosition = _debugSpawnPoint.position;
                RespawnPlayer();
                return;
            }
#endif

            // 初回はプレイヤーをスポーンさせる
            if (_isInitializeSpawn)
            {
                RespawnPlayer();
                _isInitializeSpawn = false;
            }
            else
            {
                // 初回起動時は RespawnPlayer() のタイミングで呼ばれる。
                InitializeAllAreaRespawn();
            }
        }

        private void SaveWorldStateData()
        {
#if UNITY_EDITOR
            if (IsDebug) return;
#endif
            _worldStateData.SetLastRespawnAreaId(_lastRespawnAreaId);
            JsonHandler.WriteJsonFile(WORLD_STATE_DATA_PATH, _worldStateData);
        }

        private void OnDestroy()
        {
            SaveWorldStateData();
        }

#if UNITY_EDITOR
        [ContextMenu("Respawn To Debug Point")]
        private void RespawnDebugPoint()
        {
            if (!IsDebug)
            {
                Debug.LogError("IsDebugをtrueにしてから実行してください");
                return;
            }
            if (_debugSpawnPointAreaId == "" || _debugSpawnPoint == null)
            {
                Debug.LogError("エリアID、またはスポーン地点の設定がありません。");
                return;
            }

            _lastRespawnAreaId = _debugSpawnPointAreaId;
            _respawnPosition = _debugSpawnPoint.position;
            RespawnPlayer();
        }
#endif
    }
}