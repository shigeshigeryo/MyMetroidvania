using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField, Tooltip("ドアを開く音源ファイル名")] private string _openSoundName = "SE_DoorOpen";
    SoundData _openSoundData;

    private void Start()
    {
        _openSoundData = AudioManager.Instance.GetSe(_openSoundName.GetHashCode());
    }

    public void Interact()
    {
        Open();
    }

    private void Open()
    {
        AudioManager.Instance.PlayOneShotSe(_openSoundData);
        Destroy(gameObject);
    }
}
