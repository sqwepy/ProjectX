using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaveButton : MonoBehaviour
{
    public void LeaveGame()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            // Host
            NetworkManager.singleton.StopHost();
        }
        else if (NetworkClient.isConnected)
        {
            // Client
            NetworkManager.singleton.StopClient();
        }
        else if (NetworkServer.active)
        {
            // Dedicated server
            NetworkManager.singleton.StopServer();
        }

        // Load main menu scene
        SceneManager.LoadScene("MainMenu");
    }
}
