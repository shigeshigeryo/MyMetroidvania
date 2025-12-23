using UnityEngine;

/// <summary>
/// 世界の状態を管理するスクリプト
/// 処理順は最も早くする　project settings > Script Execution Order
/// </summary>
public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance { get; private set; }

    // 世界共通の状態データ
    private WorldStateData _worldStateData;
    public WorldStateData WorldStateData => _worldStateData;

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
}
