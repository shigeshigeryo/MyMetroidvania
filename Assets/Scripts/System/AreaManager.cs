using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エリアの状態を管理するスクリプト
/// </summary>
public class AreaManager : MonoBehaviour
{
    public static Dictionary<string, AreaManager> AreaManagerList = new Dictionary<string, AreaManager>();

    [SerializeField, Tooltip("エリアID")] private string _areaId;
    public string AreaId => _areaId;
    // エリアの初期の状態データ
    private AreaStateData _areaStateData;
    public AreaStateData AreaStateData => _areaStateData;
    private string _areaStateDataPath;

#if UNITY_EDITOR
    [SerializeField, Tooltip("デバッグ用にステージ状態の更新を行いたくない場合true")]
    private bool _isDebug = false;
#endif

    private void Awake()
    {
        AreaManagerList.Add(_areaId, this);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 初期化処理
    /// 発火は初めてActiveにされた1度だけ
    /// </summary>
    private void Start()
    {
        // 特定エリアの情報を取得
        if (WorldManager.Instance.WorldStateData.TryGetAreaDataPath(_areaId, out _areaStateDataPath))
        {
            // ファイルが存在しなかった場合に初期値のファイルをロードし、
            // その値で新規にJSONファイルを作成する
            if(!JsonHandler.TryLoadJsonFile<AreaStateData>(_areaStateDataPath, out _areaStateData))
            {
                _areaStateData = JsonHandler.LoadResourcesJsonFile<AreaStateData>(_areaStateDataPath);
                SaveAreaStateData();
            }
        }

        InitializeAllGimmicks();
    }

    /// <summary>
    /// エリア内の全てのギミックを初期化
    /// </summary>
    private void InitializeAllGimmicks()
    {
        GimmickBase[] gimmicks = GetComponentsInChildren<GimmickBase>();
        foreach (var gimmick in gimmicks)
        {
            if (AreaStateData.TryGetTargetState(gimmick.Id, out var stateData))
            {
                gimmick.SetGimmickStateData(stateData);
            }
            else
            {
                Debug.LogError($"ID:{gimmick.Id} を取得できませんでした。デフォルトの初期値で処理します。");
                gimmick.SetGimmickStateData(new(gimmick.Id, 0));
            }
            gimmick.InitializeState();
        }
    }

    /// <summary>
    /// 現在のエリアの状態を保存する
    /// 各オブジェクトステートは、そのオブジェクトに紐づいているスクリプトで更新済み
    /// </summary>
    private void SaveAreaStateData()
    {
#if UNITY_EDITOR
        if (_isDebug) return;
#endif
        JsonHandler.WriteJsonFile(_areaStateDataPath, _areaStateData);
    }

    private void OnDestroy()
    {
        AreaManagerList.Remove(_areaId);

        // 更新の必要の有無をチェック
        // エリアのパスが存在しない場合は一度も訪れられていない
        if(_areaStateDataPath != default)
        {
            SaveAreaStateData();
        }
    }
}
