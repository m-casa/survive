using Mirror;
using UnityEngine;

public class CameraZone : MonoBehaviour
{
    [SerializeField] private GameObject virtualCamera;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == NetworkClient.localPlayer.gameObject)
            virtualCamera.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == NetworkClient.localPlayer.gameObject)
            virtualCamera.SetActive(false);
    }
}
