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
    public Transform playerListContainer;
    public GameObject playerListEntryPrefab;

    [Header("Buttons")]
    public Button inviteButton;
    public Button optionsButton;
    public Button leaveButton;

    private bool isPaused = false;

    public static CSteamID currentLobbyID;


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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetPauseMenu(!isPaused);
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

    // Always try to load the main menu (make sure the scene is in Build Settings!)
    SceneManager.LoadScene("MainMenu");
}

    void OpenOptionsMenu()
    {
        Debug.Log("Options menu opened (implement UI toggle here)");
        // You can expand this to show/hide an options panel
    }
}