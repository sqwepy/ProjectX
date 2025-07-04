using UnityEngine;
using Mirror;

public class CrosshairController : NetworkBehaviour
{
    [Header("Crosshair Canvas")]
    public GameObject crosshairCanvas;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        // Nur für den lokalen Spieler aktivieren
        if (crosshairCanvas != null)
        {
            crosshairCanvas.SetActive(true);
        }
    }

    void Start()
    {
        // Für alle anderen Spieler deaktivieren
        if (!isLocalPlayer && crosshairCanvas != null)
        {
            crosshairCanvas.SetActive(false);
        }
    }
}
