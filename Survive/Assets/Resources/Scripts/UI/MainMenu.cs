using HeathenEngineering.SteamworksIntegration;
using Mirror;
using Steamworks;
using System;
using System.Collections;
using TMPro;
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
    [SerializeField] private TextMeshProUGUI[] options;
    [SerializeField] private GameObject[] menuScreens;
    [SerializeField] private GameObject menuTitle;
    [SerializeField] private GameObject joinButton;
    [SerializeField] private GameObject connectingTxt;

    public static event Action EffectsChanged;

    private CameraManager cameraManager;
    private GameObject networkManager;

    /// <summary>
    /// Setup the canvas to use the main camera for screen space.
    /// </summary>

    void Awake()
    {
        cameraManager = CameraManager.Instance;

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
    /// Closes the application.
    /// </summary>

    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Changes the resolution effect in PSXEffects.
    /// 0 = Resolution
    /// </summary>

    public void ChangePsxResolution()
    {
        cameraManager.ChangeResolution();

        EffectsChanged?.Invoke();

        if (options[0].text == "LOW")
        {
            options[0].text = "HIGH";
        }
        else
        {
            options[0].text = "LOW";
        }
    }

    /// <summary>
    /// Changes the color depth effect in PSXEffects.
    /// 1 = Color Depth
    /// </summary>

    public void ChangePsxColorDepth()
    {
        cameraManager.ChangeColorDepth();

        EffectsChanged?.Invoke();

        if (options[1].text == "LOW")
        {
            options[1].text = "HIGH";
        }
        else
        {
            options[1].text = "LOW";
        }
    }

    /// <summary>
    /// Changes the scanlines effect in PSXEffects.
    /// 2 = Scanlines
    /// </summary>

    public void ChangePsxScanlines()
    {
        cameraManager.ChangeScanlines();

        EffectsChanged?.Invoke();

        if (options[2].text == "ON")
        {
            options[2].text = "OFF";
        }
        else
        {
            options[2].text = "ON";
        }
    }

    /// <summary>
    /// Changes the dithering effect in PSXEffects.
    /// 3 = Dithering
    /// </summary>

    public void ChangePsxDithering()
    {
        cameraManager.ChangeDithering();

        EffectsChanged?.Invoke();

        if (options[3].text == "ON")
        {
            options[3].text = "OFF";
        }
        else
        {
            options[3].text = "ON";
        }
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
        foreach (GameObject menuScreen in menuScreens)
        {
            menuScreen.SetActive(false);
        }

        menuTitle.SetActive(false);

        connectingTxt.SetActive(true);

        StartCoroutine(transition.ScreenFade(1.0f, 0.0f, 1.5f));

        SpawnNetworkManager();
    }

    /// <summary>
    /// What to do if we couldn't connect to the server.
    /// 0 = Main Screen, 1 = Play Screen
    /// </summary>

    private void HandleClientDisconnected()
    {
        Destroy(networkManager);

        connectingTxt.SetActive(false);
        menuTitle.SetActive(true);
        menuScreens[0].SetActive(true);

        foreach (Button button in menuScreens[1].GetComponentsInChildren<Button>())
        {
            button.interactable = true;
        }
    }
}
