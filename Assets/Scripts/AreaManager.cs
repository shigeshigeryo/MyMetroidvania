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
    private string _areaStateDataPath;

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
        if(WorldManager.Instance.WorldStateData.TryGetAreaDataPath(_areaId, out _areaStateDataPath))
        {
            _areaStateData = JsonHandler.LoadJsonFile<AreaStateData>(_areaStateDataPath);
        }
        // ファイルが存在しなかった場合に初期値のファイルをロードし、
        // その値で新規にJSONファイルを作成する
        if(_areaStateData == default)
        {
            _areaStateData = JsonHandler.LoadResourcesJsonFile<AreaStateData>(_areaStateDataPath);
            SaveAreaStateData();
        }
    }

    /// <summary>
    /// 現在のエリアの状態を保存する
    /// 各オブジェクトステートは、そのオブジェクトに紐づいているスクリプトで更新済み
    /// </summary>
    private void SaveAreaStateData()
    {
        JsonHandler.WriteJsonFile(_areaStateDataPath, _areaStateData);
    }

    private void OnDestroy()
    {
        SaveAreaStateData();
    }
}
