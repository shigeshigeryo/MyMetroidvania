using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エリアの状態を管理するスクリプト
/// WorldManagerよりも処理順を遅くする
/// </summary>
public class AreaManager : MonoBehaviour
{
    [SerializeField, Tooltip("エリアID")] private string _areaId;
    public static AreaManager Instance { get; private set; }
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
        
        // 特定エリアの情報を取得
        if(WorldManager.Instance.WorldStateData.TryGetAreaDataPath(_areaId, out var path))
        {
            _areaStateData = JsonHandler.LoadResourcesJsonFile<AreaStateData>(path);
        }
    }
}
