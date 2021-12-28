using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject virtualCamera;

    private void OnTriggerEnter(Collider other)
    {
        virtualCamera.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        virtualCamera.SetActive(false);
    }
}
