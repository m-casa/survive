using HeathenEngineering.SteamworksIntegration;
using Mirror;
using Steamworks;
using System;
using UnityEngine;

public class SurviveNetworkManager : NetworkManager
{
    public static event Action ClientDisconnected;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        // Add player
        GameObject player = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, player);

        if (SteamSettings.Initialized)
        {
            // Set this player's Steam ID and use numPlayers - 1 since
            //  we're grabbing an index, which starts counting at 0
            CSteamID cSteamId = SteamMatchmaking.GetLobbyMemberByIndex(
                SteamLobby.LobbyId,
                numPlayers - 1);

            PlayerInfo playerInfo = conn.identity.GetComponent<PlayerInfo>();

            // NOTE: This is only being set on the server's version of the player
            playerInfo.SetSteamId(cSteamId.m_SteamID);
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        // Call base functionality (actually destroys the player)
        base.OnServerDisconnect(conn);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        // If not null, call the event
        ClientDisconnected?.Invoke();
    }
}