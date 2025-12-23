using System.Collections.Generic;
using UnityEngine;

public class AreaManager : MonoBehaviour
{
    public static AreaManager Instance { get; private set; }

    [SerializeField, Tooltip("エリアID")] private string _areaId;

    // 世界共通の状態データ
    private Dictionary<string, TargetStateData> _worldStateData;
    public Dictionary<string, TargetStateData> WorldStateData => _worldStateData;
    // エリアの初期の状態データ
    private AreaStateData _areaStateData;
    public AreaStateData AreaStateData => _areaStateData;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        // 世界全ての情報
        var stateData = JsonHandler.LoadResourcesJsonFile<WorldStateData>("WorldData/WorldStateData");
        Debug.Assert(stateData != default);

        // エリア共通の情報のみ取得
        _worldStateData = stateData.AllAreaTargetStateDataList;
        // 特定エリアのみの取得
        if(stateData.TryGetAreaDataPath(_areaId, out var path))
        {
            _areaStateData = JsonHandler.LoadResourcesJsonFile<AreaStateData>(path);
        }
    }
}
