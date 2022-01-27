using UnityEngine;

public class MansionSetup : MonoBehaviour
{
    [SerializeField] private Transition _transition;

    private GameObject _cinemachineCamera;

    void Awake()
    {
        _cinemachineCamera = CameraManager.Instance.GetCinemachineCamera();
    }

    public GameObject GetCinemachineCamera()
    {
        return _cinemachineCamera;
    }

    public Transition GetTransition()
    {
        return _transition;
    }
}
