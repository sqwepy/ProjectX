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

    public static bool isMultiplayer = false; // used to decide in Lobby what to do

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
        // SteamLobby.cs will take over and load scene + start host
    }

    void PlayAlone()
    {
        isMultiplayer = false;
        SceneManager.LoadScene("Lobby");
    }
}
