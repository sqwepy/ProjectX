using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Steamworks;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenuHUD : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseMenuPanel;
    public GameObject optionsMenuPanel; // <-- NEU
    public Transform playerListContainer;
    public GameObject playerListEntryPrefab;

    [Header("Buttons")]
    public Button inviteButton;
    public Button optionsButton;
    public Button leaveButton;

    private bool isPaused = false;

    public static CSteamID currentLobbyID;

    private PlayerMovement localPlayer; // <-- NEU

    private void Start()
    {
        SetPauseMenu(false);

        if (inviteButton != null)
            inviteButton.onClick.AddListener(OpenSteamInvite);

        if (leaveButton != null)
            leaveButton.onClick.AddListener(LeaveGame);

        if (optionsButton != null)
            optionsButton.onClick.AddListener(OpenOptionsMenu);

        if (SteamManager.Initialized)
        {
            currentLobbyID = SteamLobby.currentLobbyID;
        }

        if (optionsMenuPanel != null)
            optionsMenuPanel.SetActive(false);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        if (optionsMenuPanel != null && optionsMenuPanel.activeSelf)
        {
            // Wenn Optionen offen → zurück zum Pause-Menü
            CloseOptionsMenu();
        }
        else if (pauseMenuPanel.activeSelf)
        {
            // Wenn Pause-Menü offen → schließe es
            SetPauseMenu(false);
        }
        else
        {
            // Kein Menü offen → öffne Pause-Menü
            SetPauseMenu(true);
        }
    }

    private void SetPauseMenu(bool show)
    {
        isPaused = show;
        pauseMenuPanel.SetActive(show);

        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = show;

        PlayerMovement.inputBlocked = show;

        if (show)
            RefreshPlayerList();
    }

    void RefreshPlayerList()
    {
        foreach (Transform child in playerListContainer)
        {
            Destroy(child.gameObject);
        }

        if (SteamManager.Initialized)
        {
            int count = SteamMatchmaking.GetNumLobbyMembers(currentLobbyID);

            for (int i = 0; i < count; i++)
            {
                CSteamID memberID = SteamMatchmaking.GetLobbyMemberByIndex(currentLobbyID, i);

                GameObject obj = Instantiate(playerListEntryPrefab, playerListContainer);
                SteamPlayerUI ui = obj.GetComponent<SteamPlayerUI>();
                ui.Init(memberID);
            }
        }
    }

    void OpenSteamInvite()
    {
        if (SteamManager.Initialized)
        {
            SteamFriends.ActivateGameOverlay("Friends");
        }
    }

    void LeaveGame()
    {
        Debug.Log("Attempting to leave game...");

        if (NetworkServer.active && NetworkClient.isConnected)
        {
            Debug.Log("Stopping host...");
            NetworkManager.singleton.StopHost();
        }
        else if (NetworkClient.isConnected)
        {
            Debug.Log("Stopping client...");
            NetworkManager.singleton.StopClient();
        }
        else if (NetworkServer.active)
        {
            Debug.Log("Stopping server...");
            NetworkManager.singleton.StopServer();
        }

        SceneManager.LoadScene("MainMenu");
    }

    public void SetLocalPlayer(PlayerMovement player)
    {
        localPlayer = player;
    }

    void OpenOptionsMenu()
    {
        if (optionsMenuPanel != null && localPlayer != null)
        {
            pauseMenuPanel.SetActive(false);
            optionsMenuPanel.SetActive(true);

            var options = optionsMenuPanel.GetComponent<OptionsMenu>();
            if (options != null)
                options.Open(localPlayer);
        }
    }

    void CloseOptionsMenu()
    {
        if (optionsMenuPanel != null)
            optionsMenuPanel.SetActive(false);

        pauseMenuPanel.SetActive(true);
    }
}
