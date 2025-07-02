using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Steamworks;

//public class FriendEntryUI : MonoBehaviour
//{
//    public RawImage avatarImage;
//    public TMP_Text nameText;
//    public Button inviteButton;
//    public GameObject inLobbyTag;
//
//    private CSteamID friendID;
//
//    public void SetData(CSteamID id, string name, bool inLobby)
//    {
//        friendID = id;
//        nameText.text = name;
//
//        inLobbyTag.SetActive(inLobby);
//        inviteButton.gameObject.SetActive(!inLobby);
//
//        if (!inLobby)
//        {
//            inviteButton.onClick.AddListener(() =>
//            {
//                SteamFriends.ActivateGameOverlayInviteDialog(friendID);
//            });
//        }
//
//        StartCoroutine(SteamAvatarFetcher.SetAvatarImage(avatarImage, friendID));
//    }
//}
