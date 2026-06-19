using System.Collections.Generic;
using UnityEngine;

namespace MyMetroidVania.System.Map
{
    /// <summary>
    /// エリアのマップを管理
    /// </summary>
    public class AreaMap : MonoBehaviour
    {
        public static Dictionary<string, AreaMap> AreaMapList = new();

        [SerializeField, Tooltip("エリアID")] private string _areaId;
        [SerializeField, Tooltip("エリア間のつながりを表現する線の配列")] private LineMap[] _lineMaps = null;

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Awake()
        {
            AreaMapList.Add(_areaId, this);
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 到達済み状態にする
        /// </summary>
        public void SetVisitedState()
        {
            gameObject.SetActive(true);
            // エリアとつながる線を表示状態にする
            foreach (var lineMap in _lineMaps)
            {
                lineMap.SetActive();
            }
        }

        /// <summary>
        /// clean処理
        /// </summary>
        private void OnDestroy()
        {
            AreaMapList.Remove(_areaId);
        }
    }
}
