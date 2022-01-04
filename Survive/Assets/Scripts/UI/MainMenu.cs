using HeathenEngineering.SteamworksIntegration;
using Mirror;
using Steamworks;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject lobbyButtons;
    [SerializeField] private GameObject connecting;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<LobbyEnter_t> lobbyEnter;

    void Start()
    {
        // Make sure the Steam API is available (Steam is running)
        if (!SteamSettings.Initialized) { return; }

        // These are callbacks that will run methods when a steam event has occurred
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
    }

    /// <summary>
    /// Enable any events we should listen to.
    /// </summary>

    void OnEnable()
    {
        SurviveNetworkManager.ClientDisconnected += HandleClientDisconnected;
    }

    /// <summary>
    /// Disable any events we are still listening to.
    /// </summary>

    void OnDisable()
    {
        SurviveNetworkManager.ClientDisconnected -= HandleClientDisconnected;
    }

    /// <summary>
    /// Create a lobby on our local network.
    /// Use Steam if the API is initialized
    /// </summary>

    public void HostLobby()
    {
        HandleClientConnecting();

        if (SteamSettings.Initialized)
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, NetworkManager.singleton.maxConnections);

            return;
        }

        NetworkManager.singleton.StartHost();
    }

    /// <summary>
    /// Join the lobby on our local network.
    /// </summary>

    public void JoinLocally()
    {
        HandleClientConnecting();

        NetworkManager.singleton.StartClient();
    }

    /// <summary>
    /// If our lobby is not successfully created, reset the main menu.
    /// </summary>

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            HandleClientDisconnected();
        }
    }

    /// <summary>
    /// If we successfully joined the lobby, show we're connecting.
    /// </summary>

    private void OnLobbyEnter(LobbyEnter_t callback)
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

    /// <summary>
    /// What to do if we couldn't connect to the server.
    /// </summary>

    private void HandleClientDisconnected()
    {
        title.SetActive(true);
        lobbyButtons.SetActive(true);
        connecting.SetActive(false);
    }
}
