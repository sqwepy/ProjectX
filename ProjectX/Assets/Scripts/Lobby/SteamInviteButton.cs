using UnityEngine;
using UnityEngine.UI;
using Steamworks;

public class SteamInviteButton : MonoBehaviour
{
    public Button inviteButton;

    void Start()
    {
        inviteButton.onClick.AddListener(OpenSteamInviteOverlay);
    }

    void OpenSteamInviteOverlay()
    {
        if (SteamManager.Initialized && SteamLobby.Instance != null)
        {
            var lobbyID = SteamLobby.Instance.GetCurrentLobbyID();
            if (lobbyID.IsValid())
            {
                SteamFriends.ActivateGameOverlayInviteDialog(lobbyID);
            }
            else
            {
                Debug.LogWarning("Lobby ID invalid.");
            }
        }
        else
        {
            Debug.LogWarning("Steam not initialized or SteamLobby missing.");
        }
    }
}
