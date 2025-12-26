using UnityEngine;

/// <summary>
/// 世界の状態を管理するスクリプト
/// </summary>
public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance { get; private set; }

    // 世界共通の状態データ
    private WorldStateData _worldStateData;
    public WorldStateData WorldStateData => _worldStateData;

    // 現在アクティブになっているエリア
    public AreaManager CurrentAreaManager;

    [SerializeField] private Player _player;
    private Vector3 _respawnPosition; // リスポーン地点

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
        _worldStateData = JsonHandler.LoadResourcesJsonFile<WorldStateData>("WorldData/WorldStateData");
        Debug.Assert(_worldStateData != default);
    }

    private void Start()
    {
        CurrentAreaManager = AreaManager.AreaManagerList["Area_001"]; // 仮 JSONデータから取得
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
        // エリア移動のタイミングでリスポーン場所の更新（仮）
        // TODO:リスポーン処理の変更に合わせて改修必須
        _respawnPosition = spawnPosition;
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
}
