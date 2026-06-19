using MyMetroidVania.System;
using UnityEngine;

namespace MyMetroidVania.Entity
{
    /// <summary>
    /// エリア移動で通るのゲートを管理
    /// </summary>
    public class AreaGate : MonoBehaviour
    {
        [SerializeField, Tooltip("移動先エリアのID")] private string _nextAreaId;
        [SerializeField, Tooltip("移動先エリアのプレイヤーのスポーン場所")] private Transform _spawnPoint;

        public Transform SpawnPoint => _spawnPoint;

        private static int _playerLayer = -1;
        /// <summary>
        /// プレイヤーのレイヤーを取得
        /// </summary>
        private static int PlayerLayer
        {
            get 
            { 
                if(_playerLayer == -1)
                {
                    _playerLayer = LayerMask.NameToLayer("Player");
                }
                return _playerLayer;
            }
        }

        /// <summary>
        /// トリガー処理
        /// エリア移動する
        /// </summary>
        /// <param name="collision">トリガー対象</param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == PlayerLayer)
            {
                GameManager.Instance.ChangeArea(_nextAreaId, SpawnPoint.position);
            }
        }
    }
}
