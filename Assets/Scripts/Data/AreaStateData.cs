using System.Collections.Generic;
public class AreaStateData
{
    // エリアのID 例 Area_001, Area_002
    private string _areaId;
    public string AreaId => _areaId;
    // オブジェクトの状態の管理
    // key：対象のID
    private Dictionary<string, TargetStateData> _objectStateDataList = new Dictionary<string, TargetStateData>();
    public Dictionary<string, TargetStateData> ObjectStateDataList => _objectStateDataList;

    public AreaStateData(string areaId, Dictionary<string, TargetStateData> objectStateDataList)
    {
        _areaId = areaId;
        _objectStateDataList = objectStateDataList;
    }

    /// <summary>
    /// IDで検索して存在の可否と対象の状態を返却する
    /// </summary>
    /// <param name="targetId"></param>
    /// <param name="targetSateData">ObjectStateDataを返却する</param>
    /// <returns></returns>
    public bool TryGetTargetState(string targetId, out TargetStateData targetSateData)
    {
        return ObjectStateDataList.TryGetValue(targetId, out targetSateData);
    }
}

public class TargetStateData
{
    // オブジェクトのID 例 Door_001, Door_002
    private string _targetId;
    public string TargetId => _targetId;
    // 状態管理 ステータスビットフラグで管理する予定のため、各クラスでenumに変換して使用する
    private int _state;
    public int State => _state;

    public TargetStateData(string targetId, int state)
    {
        _targetId = targetId;
        _state = state;
    }

    public void SetState(int value)
    {
        _state = value;
    }
}