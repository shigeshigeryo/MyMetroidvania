using System.Collections.Generic;

namespace MyMetroidVania.Data
{
    public class WorldStateData
    {
        // 直近のリスポーン地点のエリアのID
        private string _lastRespawnAreaId;
        public string LastRespawnAreaId => _lastRespawnAreaId;
        // 全エリア共通の状態の管理
        // key：対象のID
        private Dictionary<string, TargetStateData> _allAreaTargetStateDataList = new();
        public Dictionary<string, TargetStateData> AllAreaTargetStateDataList => _allAreaTargetStateDataList;

        // 到達したことがあるエリアのID
        private List<string> _visitedAreaIdList = new();
        public List<string> VisitedAreaIdList => _visitedAreaIdList;

        // エリアの状態の管理
        // key：対象エリアのID 
        private Dictionary<string, string> _areaStateDataPathList = new();
        public Dictionary<string, string> AreaStateDataPathList => _areaStateDataPathList;

        public WorldStateData(
            string lastRespawnAreaId,
            Dictionary<string, TargetStateData> allAreaTargetStateDataList,
            List<string> visitedAreaIdList,
            Dictionary<string, string> areaStateDataPathList)
        {
            _lastRespawnAreaId = lastRespawnAreaId;
            _allAreaTargetStateDataList = allAreaTargetStateDataList ?? new();
            _visitedAreaIdList = visitedAreaIdList ?? new();
            _areaStateDataPathList = areaStateDataPathList ?? new();
        }

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

        public void SetLastRespawnAreaId(string id)
        {
            _lastRespawnAreaId = id;
        }
    }
}