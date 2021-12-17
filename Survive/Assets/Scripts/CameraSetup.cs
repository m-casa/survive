using UnityEngine;

public class CameraSetup : MonoBehaviour
{
    [SerializeField] private GameObject[] virtualCameras;

    private GameObject cinemachineCamera;

    void Start()
    {
        cinemachineCamera = CameraManager.Instance.GetCinemachineCamera();

        foreach (GameObject cam in virtualCameras)
        {
            cam.transform.parent = cinemachineCamera.transform;
        }
    }
}
