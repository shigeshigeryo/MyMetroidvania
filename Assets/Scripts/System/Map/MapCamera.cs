using UnityEngine;

public class MapCamera : MonoBehaviour
{
    [SerializeField] private Transform _playerMarker;

    private void LateUpdate()
    {
        transform.position
            = new Vector3(_playerMarker.position.x, _playerMarker.position.y, transform.position.z);
    }
}
