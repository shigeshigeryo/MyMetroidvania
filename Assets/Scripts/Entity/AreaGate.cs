using MyMetroidVania.System;
using UnityEngine;

namespace MyMetroidVania.Entity
{
    public class AreaGate : MonoBehaviour
    {
        [SerializeField, Tooltip("移動先エリアのID")] private string _nextAreaId;
        public string NextAreaId => _nextAreaId;
        [SerializeField, Tooltip("移動先エリアのプレイヤーのスポーン場所")] private Transform _spawnPoint;
        public Transform SpawnPoint => _spawnPoint;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                WorldManager.Instance.ChangeArea(_nextAreaId, SpawnPoint.position);
            }
        }
    }
}