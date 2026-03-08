using MyMetroidVania.System;
using UnityEngine;

public class ClearGate : MonoBehaviour
{
    private static int _playerLayer = -1;
    // LazyInit
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == PlayerLayer)
        {
            GameManager.Instance.GameClear();
        }
    }
}
