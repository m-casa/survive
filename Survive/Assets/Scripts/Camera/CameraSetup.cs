using UnityEngine;

public class CameraSetup : MonoBehaviour
{
    [SerializeField] private GameObject[] virtualCameras;

    private GameObject cinemachineCamera;

    void Awake()
    {
        cinemachineCamera = GetComponentInParent<MansionSetup>().GetCinemachineCamera();

        foreach (GameObject virtualCamera in virtualCameras)
        {
            virtualCamera.transform.parent = cinemachineCamera.transform;
        }
    }
}
