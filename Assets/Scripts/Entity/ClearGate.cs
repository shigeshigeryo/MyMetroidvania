using MyMetroidVania.System;
using UnityEngine;

namespace MyMetroidVania.Entity
{
    /// <summary>
    /// クリアのゲートを管理
    /// </summary>
    public class ClearGate : MonoBehaviour
    {
        private static int _playerLayer = -1;
        /// <summary>
        /// プレイヤーのレイヤーを取得
        /// </summary>
        private static int PlayerLayer
        {
            get
            {
                if (_playerLayer == -1)
                {
                    _playerLayer = LayerMask.NameToLayer("Player");
                }
                return _playerLayer;
            }
        }

        /// <summary>
        /// トリガー処理
        /// クリアする
        /// </summary>
        /// <param name="collision">トリガー対象</param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == PlayerLayer)
            {
                GameManager.Instance.GameClear();
            }
        }
    }
}
