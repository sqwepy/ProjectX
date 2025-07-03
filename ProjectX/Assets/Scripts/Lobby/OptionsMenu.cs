using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    [Header("References")]
    public GameObject optionsPanel;
    public Slider moveSpeedSlider;
    public Slider jumpForceSlider;
    public Slider sensitivitySlider;

    public TextMeshProUGUI moveSpeedText;
    public TextMeshProUGUI jumpForceText;
    public TextMeshProUGUI sensitivityText;

    private PlayerMovement localPlayer;

    void Start()
    {
        optionsPanel.SetActive(false); // Start hidden

        // Optionale Default-Werte setzen
        moveSpeedSlider.minValue = 1000;
        moveSpeedSlider.maxValue = 10000;
        jumpForceSlider.minValue = 0f;
        jumpForceSlider.maxValue = 20f;
        sensitivitySlider.minValue = 1f;
        sensitivitySlider.maxValue = 200f;

        moveSpeedSlider.onValueChanged.AddListener(UpdateMoveSpeed);
        jumpForceSlider.onValueChanged.AddListener(UpdateJumpForce);
        sensitivitySlider.onValueChanged.AddListener(UpdateSensitivity);
    }

    public void Open(PlayerMovement player)
    {
        localPlayer = player;
        if (!localPlayer) return;

        // Werte in UI laden
        moveSpeedSlider.value = localPlayer.moveSpeed;
        jumpForceSlider.value = localPlayer.jumpForce;
        sensitivitySlider.value = localPlayer.sensitivity;

        UpdateTexts();

        optionsPanel.SetActive(true);
    }

    public void Close()
    {
        optionsPanel.SetActive(false);
    }

    private void UpdateMoveSpeed(float value)
    {
        if (localPlayer) localPlayer.moveSpeed = value;
        UpdateTexts();
    }

    private void UpdateJumpForce(float value)
    {
        if (localPlayer) localPlayer.jumpForce = value;
        UpdateTexts();
    }

    private void UpdateSensitivity(float value)
    {
        if (localPlayer) localPlayer.sensitivity = value;
        UpdateTexts();
    }

    private void UpdateTexts()
    {
        moveSpeedText.text = $"Speed: {moveSpeedSlider.value:F0}";
        jumpForceText.text = $"Jump: {jumpForceSlider.value:F1}";
        sensitivityText.text = $"Sens: {sensitivitySlider.value:F1}";
    }
}
