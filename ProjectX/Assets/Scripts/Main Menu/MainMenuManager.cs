using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using Steamworks;

public class MainMenuManager : MonoBehaviour
{
    public TextMeshProUGUI steamNameText;
    public Button hostGameButton;
    public Button playAloneButton;

    public GameObject playerPrefab;
    public Transform spawnPoint;

    public static bool isMultiplayer = false;

    private void Start()
    {
        if (steamNameText != null)
        {
            steamNameText.text = SteamManager.Initialized
                ? $"Steam: {SteamFriends.GetPersonaName()}"
                : "Steam nicht verfügbar";
        }

        hostGameButton.onClick.AddListener(HostGame);
        playAloneButton.onClick.AddListener(PlayAlone);
    }

    void HostGame()
    {
        isMultiplayer = true;
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
    }

    void PlayAlone()
    {
        isMultiplayer = false;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("Lobby");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Lobby")
        {
            Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
