using UnityEngine;

public class MansionSetup : MonoBehaviour
{
    [SerializeField] private GameObject _cinemachineCamera;
    [SerializeField] private Transition _transition;

    public GameObject GetCinemachineCamera()
    {
        return _cinemachineCamera;
    }

    public Transition GetTransition()
    {
        return _transition;
    }
}
