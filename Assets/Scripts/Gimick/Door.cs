using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Open();
    }

    private void Open()
    {
        Destroy(gameObject);
    }
}
