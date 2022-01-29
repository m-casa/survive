using HeathenEngineering.SteamworksIntegration;
using Mirror;
using Steamworks;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Network Managers")]
    [SerializeField] private GameObject networkManagerPrefab;
    [SerializeField] private GameObject steamNetworkManagerPrefab;

    [Header("UI")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private Transition transition;
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject lobbyButtons;
    [SerializeField] private GameObject joinButton;
    [SerializeField] private GameObject connectingTxt;

    private GameObject networkManager;

    /// <summary>
    /// Setup the canvas to use the main camera for screen space.
    /// </summary>

    void Awake()
    {
        canvas.worldCamera = Camera.main;
        canvas.planeDistance = 0.12f;
    }

    /// <summary>
    /// Steam API will initialize after Awake, so check for the API on Start.
    /// </summary>

    void Start()
    {
        // Make sure the Steam API is available (Steam is running)
        if (!SteamSettings.Initialized) { return; }

        joinButton.SetActive(false);
    }

    /// <summary>
    /// Enable any events we should listen to.
    /// </summary>

    void OnEnable()
    {
        SurviveNetworkManager.ClientDisconnected += HandleClientDisconnected;

        SteamLogic.LobbyFailed += HandleClientDisconnected;
        SteamLogic.LobbyJoined += HandleClientConnecting;
    }

    /// <summary>
    /// Disable any events we are still listening to.
    /// </summary>

    void OnDisable()
    {
        SurviveNetworkManager.ClientDisconnected -= HandleClientDisconnected;

        SteamLogic.LobbyFailed -= HandleClientDisconnected;
        SteamLogic.LobbyJoined -= HandleClientConnecting;
    }

    /// <summary>
    /// Calls the Host coroutine.
    /// </summary>
    
    public void HostGame()
    {
        StartCoroutine(Host());
    }

    /// <summary>
    /// Calls the Join coroutine.
    /// </summary>

    public void JoinGame()
    {
        StartCoroutine(Join());
    }

    /// <summary>
    /// Host a lobby on our local network.
    /// Use Steam if the API is initialized
    /// </summary>

    private IEnumerator Host()
    {
        yield return StartCoroutine(transition.SceneChange());

        HandleClientConnecting();

        if (SteamSettings.Initialized)
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, NetworkManager.singleton.maxConnections);

            yield break;
        }

        NetworkManager.singleton.StartHost();
    }

    /// <summary>
    /// Join the lobby on our local network.
    /// </summary>

    private IEnumerator Join()
    {
        yield return StartCoroutine(transition.SceneChange());

        HandleClientConnecting();

        NetworkManager.singleton.StartClient();
    }

    /// <summary>
    /// Spawns the appropriate Network Manager.
    /// </summary>

    private void SpawnNetworkManager()
    {
        if (SteamSettings.Initialized)
        {
            networkManager = Instantiate(steamNetworkManagerPrefab);
        }
        else
        {
            networkManager = Instantiate(networkManagerPrefab);
        }
    }

    /// <summary>
    /// What to do when connecting to the server.
    /// </summary>

    private void HandleClientConnecting()
    {
        StartCoroutine(transition.ScreenFade(1.0f, 0.0f, 1.5f));

        title.SetActive(false);
        lobbyButtons.SetActive(false);
        connectingTxt.SetActive(true);

        SpawnNetworkManager();
    }

    /// <summary>
    /// What to do if we couldn't connect to the server.
    /// </summary>

    private void HandleClientDisconnected()
    {
        Destroy(networkManager);

        connectingTxt.SetActive(false);
        title.SetActive(true);
        lobbyButtons.SetActive(true);

        foreach (Button button in lobbyButtons.GetComponentsInChildren<Button>())
        {
            button.interactable = true;
        }
    }
}
