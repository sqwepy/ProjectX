using Mirror;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LobbyPlayerList : MonoBehaviour
{
    public GameObject playerEntryPrefab;
    public Transform playerListContainer;

    void OnEnable()
    {
        RefreshPlayerList();
    }

    public void RefreshPlayerList()
    {
        foreach (Transform child in playerListContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {
            if (conn.identity != null)
            {
                string playerName = conn.identity.GetComponent<NetworkIdentity>().gameObject.name;
                GameObject entry = Instantiate(playerEntryPrefab, playerListContainer);
                entry.GetComponentInChildren<Text>().text = playerName;
            }
        }
    }
}
