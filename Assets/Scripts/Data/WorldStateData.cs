using System.Collections.Generic;

public class WorldStateData
{
    // 全エリア共通の状態の管理
    // key：対象のID
    private Dictionary<string, TargetStateData> _allAreaTargetStateDataList = new Dictionary<string, TargetStateData>();
    public Dictionary<string, TargetStateData> AllAreaTargetStateDataList => _allAreaTargetStateDataList;

    // エリアの状態の管理
    // key：対象エリアのID 
    private Dictionary<string, string> _areaStateDataPathList = new Dictionary<string, string>();
    public Dictionary<string, string> AreaStateDataPathList => _areaStateDataPathList;

    /// <summary>
    /// IDで検索して存在の可否とエリアの状態データを返却する（全てのエリア共通の対象）
    /// </summary>
    /// <param name="areaId"></param>
    /// <param name="areaStateData">ObjectStateDataを返却する</param>
    /// <returns></returns>
    public bool TryGetAllAreaTargetState(string targetId, out TargetStateData targetSateData)
    {
        return AllAreaTargetStateDataList.TryGetValue(targetId, out targetSateData);
    }

    /// <summary>
    /// エリアIDで検索して存在の可否とエリアの状態データを保存しているパスを返却する
    /// </summary>
    /// <param name="areaId"></param>
    /// <param name="path">ObjectStateDataを返却する</param>
    /// <returns></returns>
    public bool TryGetAreaDataPath(string areaId, out string path)
    {
        return AreaStateDataPathList.TryGetValue(areaId, out path);
    }
}
