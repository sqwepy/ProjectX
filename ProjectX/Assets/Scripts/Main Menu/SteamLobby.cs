using UnityEngine;
using Steamworks;
using Mirror;
using UnityEngine.SceneManagement;
using Mirror.FizzySteam;

public class SteamLobby : MonoBehaviour
{
    public static SteamLobby Instance { get; private set; }

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> joinRequest;
    protected Callback<LobbyEnter_t> lobbyEntered;

    private const string HostAddressKey = "HostAddress";
    private CSteamID currentLobbyID;

    private void Awake()
    {
        // Make singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogWarning("Steam is not initialized!");
            return;
        }

        // Register callbacks
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        joinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
    }

    private void OnLobbyCreated(LobbyCreated_t result)
    {
        if (result.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError("Failed to create Steam lobby.");
            return;
        }

        currentLobbyID = new CSteamID(result.m_ulSteamIDLobby);

        // Store host Steam ID so others can find host
        SteamMatchmaking.SetLobbyData(currentLobbyID, HostAddressKey, SteamUser.GetSteamID().ToString());

        // Load Lobby scene (optional)
        SceneManager.LoadScene("Lobby");

        // Start Mirror host
        NetworkManager.singleton.StartHost();
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t request)
    {
        Debug.Log("Received Steam invite. Joining lobby...");
        SteamMatchmaking.JoinLobby(request.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t result)
    {
        if (NetworkServer.active) return; // already host

        currentLobbyID = new CSteamID(result.m_ulSteamIDLobby);

        // Read host's SteamID from lobby data
        string hostSteamID = SteamMatchmaking.GetLobbyData(currentLobbyID, HostAddressKey);
        if (string.IsNullOrEmpty(hostSteamID))
        {
            Debug.LogError("HostAddress not found in lobby data!");
            return;
        }

        // ✅ Set the SteamID for the transport manually
        NetworkManager.singleton.StartClient();

        // Load the Lobby scene (if needed)
        SceneManager.LoadScene("Lobby");

        // ✅ Now connect
        NetworkManager.singleton.StartClient();
    }

    public CSteamID GetCurrentLobbyID()
    {
        return currentLobbyID;
    }

    // Optional: Call from a button to open Steam invite overlay
    public void OpenInviteOverlay()
    {
        if (SteamManager.Initialized && currentLobbyID.IsValid())
        {
            SteamFriends.ActivateGameOverlayInviteDialog(currentLobbyID);
        }
    }
}
