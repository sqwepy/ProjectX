using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pauseMenuPanel;
    public GameObject optionsMenuPanel;

    private PlayerMovement localPlayer;

    public void SetPlayer(PlayerMovement player)
    {
        localPlayer = player;
    }

    private void Start()
    {
        pauseMenuPanel.SetActive(false);
        optionsMenuPanel.SetActive(false);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        if (optionsMenuPanel.activeSelf)
        {
            // Zurück aus Optionen → zurück ins Pause-Menü
            OnBackFromOptions();
        }
        else if (pauseMenuPanel.activeSelf)
        {
            // Pause-Menü schließen
            pauseMenuPanel.SetActive(false);
        }
        else
        {
            // Pause-Menü öffnen
            pauseMenuPanel.SetActive(true);
        }
    }

    public void OnOptionsButtonClicked()
    {
        if (optionsMenuPanel != null && localPlayer != null)
        {
            optionsMenuPanel.SetActive(true);
            pauseMenuPanel.SetActive(false);

            var options = optionsMenuPanel.GetComponent<OptionsMenu>();
            if (options != null)
            {
                options.Open(localPlayer);
            }
        }
    }

    public void OnBackFromOptions()
    {
        optionsMenuPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }
}
