using UnityEngine;
using TMPro;

public class NameTag : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Transform targetPlayer;
    public Transform orientation;  // Reference to the player's orientation object

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void LateUpdate()
    {
        if (targetPlayer == null || orientation == null || mainCam == null)
            return;

        // Match rotation of the player's orientation (so it rotates with them)
        transform.rotation = orientation.rotation;

        // Make the text always face the camera (billboard)
        transform.LookAt(transform.position + mainCam.transform.rotation * Vector3.forward,
                         mainCam.transform.rotation * Vector3.up);
    }

    public void SetName(string name)
    {
        nameText.text = name;
    }
}
