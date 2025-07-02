//using UnityEngine;
//using UnityEngine.UI;
//using Steamworks;
//using System.Collections.Generic;
//
//public class LobbyFriendList : MonoBehaviour
//{
//    public GameObject friendEntryPrefab;
//    public Transform friendListContainer;
//
//    private CSteamID lobbyID;
//    private List<CSteamID> lobbyMembers = new List<CSteamID>();
//
//    void Start()
//    {
//        if (!SteamManager.Initialized) return;
//
//        lobbyID = SteamLobby.Instance.GetCurrentLobbyID();
//        FetchLobbyMembers();
//        PopulateFriendList();
//    }
//
//    void FetchLobbyMembers()
//    {
//        int count = SteamMatchmaking.GetNumLobbyMembers(lobbyID);
//        for (int i = 0; i < count; i++)
//        {
//            CSteamID member = SteamMatchmaking.GetLobbyMemberByIndex(lobbyID, i);
//            lobbyMembers.Add(member);
//        }
//    }
//
//    void PopulateFriendList()
//    {
//        int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
//
//        for (int i = 0; i < friendCount; i++)
//        {
//            CSteamID friendID = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
//            string name = SteamFriends.GetFriendPersonaName(friendID);
//
//            GameObject entry = Instantiate(friendEntryPrefab, friendListContainer);
//            FriendEntryUI entryUI = entry.GetComponent<FriendEntryUI>();
//            entryUI.SetData(friendID, name, lobbyMembers.Contains(friendID));
//        }
//    }
//}