using UnityEngine;
using Facepunch.Steamworks;
using Mirror;
using UnityEngine.SceneManagement;

public class SteamLobby : MonoBehaviour
{
    public static SteamLobby Instance { get; private set; }

    private Lobby currentLobby;
    private Client steamClient;

    public ulong LobbyID => currentLobby?.Id ?? 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        steamClient = new Client(AppId.YourAppIdHere);

        if (!steamClient.IsValid)
        {
            Debug.LogError("Steam client not initialized.");
            return;
        }

        steamClient.Lobby.OnLobbyCreated += OnLobbyCreated;
        steamClient.Lobby.OnLobbyEntered += OnLobbyEntered;
        steamClient.Lobby.OnLobbyInvite += OnLobbyInvite;
    }

    public void HostLobby()
    {
        steamClient.Lobby.Create(Lobby.Type.FriendsOnly, 4);
    }

    private void OnLobbyCreated(bool success, Lobby lobby)
    {
        if (!success)
        {
            Debug.LogError("Failed to create lobby.");
            return;
        }

        currentLobby = lobby;
        currentLobby.SetData("HostAddress", steamClient.SteamId.ToString());

        SceneManager.LoadScene("Lobby");
        NetworkManager.singleton.StartHost();
    }

    private void OnLobbyEntered(Lobby lobby)
    {
        currentLobby = lobby;
        Debug.Log("Joined Lobby: " + lobby.Id);

        if (NetworkServer.active) return;

        string hostId = currentLobby.GetData("HostAddress");
        if (string.IsNullOrEmpty(hostId))
        {
            Debug.LogError("No host address found in lobby!");
            return;
        }

        // Optional: set address, not needed for Steam transport
        // NetworkManager.singleton.networkAddress = hostId;

        SceneManager.LoadScene("Lobby");
        NetworkManager.singleton.StartClient();
    }

    private void OnLobbyInvite(ulong lobbyId)
    {
        Debug.Log("Received lobby invite. Joining...");
        steamClient.Lobby.Join(lobbyId);
    }

    public void OpenInviteOverlay()
    {
        if (steamClient?.Lobby?.Current != null)
        {
            steamClient.Friends.OpenOverlayInviteDialog(currentLobby);
        }
    }

    private void OnDestroy()
    {
        steamClient?.Dispose();
    }
}
