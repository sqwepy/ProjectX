using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetcodeUI : MonoBehaviour
{
    public Button hostButton;
    public Button clientButton;
    public Button serverButton;

    void Start()
    {
        hostButton.onClick.AddListener(() => StartHost());
        clientButton.onClick.AddListener(() => StartClient());
        serverButton.onClick.AddListener(() => StartServer());
    }

    void StartHost()
    {
        Debug.Log("Starting as Host...");
        NetworkManager.Singleton.StartHost();
    }

    void StartClient()
    {
        Debug.Log("Starting as Client...");
        NetworkManager.Singleton.StartClient();
    }

    void StartServer()
    {
        Debug.Log("Starting as Server...");
        NetworkManager.Singleton.StartServer();
    }
}
