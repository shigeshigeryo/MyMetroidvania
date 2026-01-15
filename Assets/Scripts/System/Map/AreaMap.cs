using System.Collections.Generic;
using UnityEngine;

namespace MyMetroidVania.System.Map
{
    public class AreaMap : MonoBehaviour
    {
        public static Dictionary<string, AreaMap> AreaMapList = new();

        [SerializeField, Tooltip("エリアID")] private string _areaId;
        [SerializeField, Tooltip("エリア間のつながりを表現する線の配列")] private LineMap[] _lineMaps = null;
        void Start()
        {
            AreaMapList.Add(_areaId, this);
            gameObject.SetActive(false);
        }

        // 到達済み状態にする（踏破状態も作る?）
        public void SetVisitedState()
        {
            gameObject.SetActive(true);
            // エリアとつながる線を表示状態にする
            foreach (var lineMap in _lineMaps)
            {
                lineMap.SetActive();
            }
        }

        private void OnDestroy()
        {
            AreaMapList.Remove(_areaId);
        }
    }
}