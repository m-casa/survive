using UnityEngine;

public class CameraSetup : MonoBehaviour
{
    [SerializeField] private GameObject[] virtualCameras;

    private GameObject cinemachineCamera;

    void Awake()
    {
        cinemachineCamera = CameraManager.Instance.GetCinemachineCamera();
    }

    void Start()
    {
        foreach (GameObject cam in virtualCameras)
        {
            cam.transform.parent = cinemachineCamera.transform;
        }
    }
}
