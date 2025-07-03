using UnityEngine;
using Steamworks;

public class SteamInvite : MonoBehaviour
{
    public void OpenInviteOverlay()
    {
        if (SteamManager.Initialized)
        {
            SteamFriends.ActivateGameOverlay("Friends");
        }
    }
}
