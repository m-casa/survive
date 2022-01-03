using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject lobbyButtons;
    [SerializeField] private GameObject connecting;

    /// <summary>
    /// Create a lobby on our local network.
    /// Use Steam if the API is initialized
    /// </summary>

    public void HostLobby()
    {
        HandleClientConnecting();
    }

    /// <summary>
    /// What to do when connecting to the server.
    /// </summary>

    private void HandleClientConnecting()
    {
        title.SetActive(false);
        lobbyButtons.SetActive(false);
        connecting.SetActive(true);
    }
}
