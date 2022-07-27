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
        foreach (GameObject virtualCamera in virtualCameras)
        {
            virtualCamera.transform.parent = cinemachineCamera.transform;
        }
    }
}
