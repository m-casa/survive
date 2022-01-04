using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    private const string MainMenu = "OfflineScene";

    void Start()
    {
        SceneManager.LoadScene(MainMenu);
    }
}
