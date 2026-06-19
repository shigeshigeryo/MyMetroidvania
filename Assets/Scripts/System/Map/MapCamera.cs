using UnityEngine;

namespace MyMetroidVania.System.Map
{
    /// <summary>
    /// ミニマップを表示するカメラを管理
    /// </summary>
    public class MapCamera : MonoBehaviour
    {
        [SerializeField] private Transform _playerMarker;

        /// <summary>
        /// プレイヤーマーカーのポジションにセットする
        /// </summary>
        private void LateUpdate()
        {
            transform.position
                = new Vector3(_playerMarker.position.x, _playerMarker.position.y, -10);
        }
    }
}
