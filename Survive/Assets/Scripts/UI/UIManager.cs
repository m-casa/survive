using Animancer;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private NamedAnimancerComponent crossfadeAnimancer;

    private void Awake()
    {
        // When our new scene loads, don't delete the UI manager
        DontDestroyOnLoad(gameObject);

        // Check if we have an instance of the UI manager
        if (Instance != null)
        {
            // If we already have a UI manager, destroy this one
            Destroy(gameObject);
        }

        // Set this UI manager as the primary instance since we don't have one
        Instance = this;
    }

    public void StartCrossfade()
    {
        crossfadeAnimancer.TryPlay("Start");
    }

    public void EndCrossfade()
    {
        crossfadeAnimancer.TryPlay("End");
    }
}
