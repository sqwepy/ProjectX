using UnityEngine;

public class PauseMenuToggle : MonoBehaviour
{
    [Header("UI")]
    public GameObject pauseMenuUI;

    private bool isPaused;

    void Start()
    {
        SetPauseMenu(false); // Hide menu on start
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetPauseMenu(!isPaused);
        }
    }

    private void SetPauseMenu(bool show)
    {
        isPaused = show;
        pauseMenuUI.SetActive(show);

        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = show;

        // Optional: Freeze gameplay time if needed
        // Time.timeScale = show ? 0f : 1f;
    }
}
