using UnityEngine;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private Canvas canvas;

    /// <summary>
    /// Setup the canvas to use the main camera for screen space.
    /// </summary>

    void Awake()
    {
        canvas.worldCamera = Camera.main;
        canvas.planeDistance = 0.1f;
    }
}
