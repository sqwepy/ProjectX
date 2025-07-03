using UnityEngine;
using Steamworks;

public class LobbyUIManager : MonoBehaviour
{
    [Header("Prefabs & UI")]
    public GameObject playerUIPrefab;          // Your Friend.prefab
    public Transform playerListContainer;      // Where to spawn the entries (e.g. Vertical Layout Group)

    private CSteamID lobbyID;

    private void Start()
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogWarning("Steam not initialized");
            return;
        }

        lobbyID = (CSteamID)SteamMatchmaking.GetLobbyOwner(SteamMatchmaking.GetLobbyByIndex(0)); // or store your actual lobbyID

        UpdateLobbyUI();
    }

    public void UpdateLobbyUI()
    {
        // Clean up previous entries
        foreach (Transform child in playerListContainer)
        {
            Destroy(child.gameObject);
        }

        int memberCount = SteamMatchmaking.GetNumLobbyMembers(lobbyID);

        for (int i = 0; i < memberCount; i++)
        {
            CSteamID memberID = SteamMatchmaking.GetLobbyMemberByIndex(lobbyID, i);

            GameObject uiObj = Instantiate(playerUIPrefab, playerListContainer);
            SteamPlayerUI ui = uiObj.GetComponent<SteamPlayerUI>();
            ui.Init(memberID);
        }
    }
}
