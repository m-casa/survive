using HeathenEngineering.SteamworksIntegration;
using Mirror;
using Steamworks;
using UnityEngine;

[RequireComponent(typeof(NetworkManager))]
public class SteamLobby : MonoBehaviour
{
    public static CSteamID LobbyId { get; private set; }

    private const string HostAddressKey = "HostAddress";

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEnter;
    protected Callback<LobbyChatUpdate_t> lobbyChatUpdate;
    protected Callback<GameOverlayActivated_t> gameOverlayActivated;

    /// <summary>
    /// Steam API will initialize after Awake, so check for the API on Start.
    /// </summary>

    void Start()
    {
        // Make sure the Steam API is available (Steam is running)
        if (!SteamSettings.Initialized) { return; }

        // These are callbacks that will run methods when a steam event has occurred
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
        lobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
        gameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
    }

    /// <summary>
    /// If our lobby is successfully created, host a server on our local network.
    /// </summary>
    
    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        // Make sure the lobby was successfully created
        if (callback.m_eResult != EResult.k_EResultOK) { return; }

        LobbyId = new CSteamID(callback.m_ulSteamIDLobby);

        // Since other players don't join by IP but rather by the host's Steam ID,
        //  we can ask Steam to keep track of our ID by associating it with HostAddressKey
        SteamMatchmaking.SetLobbyData(
            LobbyId,
            HostAddressKey,
            SteamUser.GetSteamID().ToString());

        NetworkManager.singleton.StartHost();
    }

    /// <summary>
    /// Called when we request to join a lobby.
    /// </summary>
   
    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        if (NetworkServer.active) 
        {
            // If we're already hosting a server, stop the server
            NetworkManager.singleton.StopHost();
        }
        else if (NetworkClient.active)
        {
            // If we're already in a game, stop the client
            NetworkManager.singleton.StopClient();;
        }

        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    /// <summary>
    /// If we successfully joined the lobby, start the client.
    /// </summary>
    
    private void OnLobbyEnter(LobbyEnter_t callback)
    {
        // Since the host will also receive the OnLobbyEnter callback,
        //  have them ignore this part (they're not the one entering)
        if (NetworkServer.active) { return; }

        // Retrieve the host's Steam ID we associated with
        //  HostAddressKey when the lobby was created
        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby), 
            HostAddressKey);

        NetworkManager.singleton.networkAddress = hostAddress;
        NetworkManager.singleton.StartClient();
    }

    /// <summary>
    /// Called when a player joins/leaves the lobby.
    /// </summary>
    
    private void OnLobbyChatUpdate(LobbyChatUpdate_t callback)
    {
        // Check if the callback is someone leaving the lobby
        if (callback.m_rgfChatMemberStateChange == ((uint)EChatMemberStateChange.k_EChatMemberStateChangeLeft))
        {
            string hostAddress = SteamMatchmaking.GetLobbyData(
                new CSteamID(callback.m_ulSteamIDLobby),
                HostAddressKey);

            // If the host left, we should also leave the lobby
            if (callback.m_ulSteamIDUserChanged.ToString() == hostAddress)
                SteamMatchmaking.LeaveLobby(new CSteamID(callback.m_ulSteamIDLobby));
        }
    }

    protected virtual void OnGameOverlayActivated(GameOverlayActivated_t callback)
    {
        if (callback.m_bActive == 1)
        {

        }
        else
        {

        }
    }
}