using System.Collections.Generic;
using UnityEngine;

namespace MyMetroidVania.System.Map
{
    /// <summary>
    /// 全てのエリアマップを管理
    /// </summary>
    public class MapManager : MonoBehaviour
    {
        public static MapManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize(List<string> visitedAreaIdList)
        {
            foreach (var areaId in visitedAreaIdList)
            {
                SetVisitedArea(areaId);
            }
        }

        /// <summary>
        /// 訪れたことのあるエリアをアクティブ状態にする
        /// </summary>
        public void SetVisitedArea(string areaId)
        {
            if (AreaMap.AreaMapList.ContainsKey(areaId))
            {
                AreaMap.AreaMapList[areaId].SetVisitedState();
            }
        }
    }
}
