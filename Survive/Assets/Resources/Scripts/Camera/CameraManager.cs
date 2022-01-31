using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [SerializeField] private GameObject _cinemachineCamera;

    [SerializeField] private PSXEffects _psxEffects;

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

    void OnEnable()
    {
        SceneManager.activeSceneChanged += ResetCinemachineCamera;

        MainMenu.EffectsChanged += UpdateEffects;
    }

    void OnDisable()
    {
        SceneManager.activeSceneChanged -= ResetCinemachineCamera;

        MainMenu.EffectsChanged -= UpdateEffects;
    }

    public GameObject GetCinemachineCamera()
    {
        return _cinemachineCamera;
    }

    public void ChangeResolution()
    {
        if (_psxEffects.resolutionFactor == 3)
        {
            _psxEffects.resolutionFactor = 1;
        }
        else
        {
            _psxEffects.resolutionFactor = 3;
        }
    }

    public void ChangeColorDepth()
    {
        if (_psxEffects.colorDepth == 5)
        {
            _psxEffects.colorDepth = 24;
        }
        else
        {
            _psxEffects.colorDepth = 5;
        }
    }

    public void ChangeScanlines()
    {
        if (_psxEffects.scanlines)
        {
            _psxEffects.scanlines = false;
        }
        else
        {
            _psxEffects.scanlines = true;
        }
    }

    public void ChangeDithering()
    {
        if (_psxEffects.dithering)
        {
            _psxEffects.dithering = false;
        }
        else
        {
            _psxEffects.dithering = true;
        }
    }

    private void UpdateEffects()
    {
        _psxEffects.UpdateProperties();
    }

    private void ResetCinemachineCamera(Scene current, Scene next)
    {
        foreach (Transform virtualCamera in _cinemachineCamera.transform)
        {
            Destroy(virtualCamera.gameObject);
        }
    }
}
