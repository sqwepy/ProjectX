using UnityEngine;
using Mirror;

public class LobbySceneManager : MonoBehaviour
{
    public NetworkManager networkManager;

    void Start()
    {
        if (MainMenuManager.isMultiplayer)
        {
            // Multiplayer logic → handled already by SteamLobby.cs
            Debug.Log("Multiplayer Lobby - Steam should handle host/join");
        }
        else
        {
            // Singleplayer → don't start Mirror networking
            if (NetworkClient.active || NetworkServer.active)
            {
                networkManager.StopHost(); // if any previous session somehow leaked in
            }

            Debug.Log("Singleplayer Lobby - no network");
        }
    }
}
