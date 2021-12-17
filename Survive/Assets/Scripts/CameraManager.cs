using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [SerializeField] private GameObject _cinemachineCamera;

    void Awake()
    {
        // When our new scene loads, don't delete the camera manager
        DontDestroyOnLoad(gameObject);

        // Check if we have an instance of the camera manager
        if (Instance != null)
        {
            // If we already have a camera manager, destroy this one
            Destroy(gameObject);
        }

        // Set this camera manager as the primary instance since we don't have one
        Instance = this;
    }

    public GameObject GetCinemachineCamera()
    {
        return _cinemachineCamera;
    }
}
