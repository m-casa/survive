using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private GameObject _cinemachineCamera;
    [SerializeField] private CanvasGroup _crossfadeGroup;

    public GameObject GetCinemachineCamera()
    {
        return _cinemachineCamera;
    }

    public CanvasGroup GetCrossfadeGroup()
    {
        return _crossfadeGroup;
    }
}
