using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        // When our new scene loads, don't delete the game manager
        DontDestroyOnLoad(gameObject);

        // Check if we have an instance of the game manager
        if (Instance != null)
        {
            // If we already have a game manager, destroy this one
            Destroy(gameObject);
        }

        // Set this game manager as the primary instance since we don't have one
        Instance = this;
    }
}
