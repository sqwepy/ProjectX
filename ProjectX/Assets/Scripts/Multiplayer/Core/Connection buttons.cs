using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.EventSystems;

public class NetcodeButtonUI : MonoBehaviour
{
    private Canvas canvas;

    void Start()
    {
        // Create canvas
        GameObject canvasObj = new GameObject("Canvas");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // Ensure EventSystem exists
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        // Create buttons
        CreateButton("Host", new Vector2(0, 100), StartHost);
        CreateButton("Client", new Vector2(0, 0), StartClient);
        CreateButton("Server", new Vector2(0, -100), StartServer);
    }

    void CreateButton(string label, Vector2 position, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObj = new GameObject(label + "Button", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObj.transform.SetParent(canvas.transform, false);

        RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(160, 40);
        rectTransform.anchoredPosition = position;

        Button button = buttonObj.GetComponent<Button>();
        button.onClick.AddListener(action);

        GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textObj.transform.SetParent(buttonObj.transform, false);

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.sizeDelta = rectTransform.sizeDelta;
        textRect.anchoredPosition = Vector2.zero;

        Text text = textObj.GetComponent<Text>();
        text.text = label;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.black;
        text.fontSize = 20;
    }

    void StartHost()
    {
        Debug.Log("Starting Host...");
        NetworkManager.Singleton.StartHost();
    }

    void StartClient()
    {
        Debug.Log("Starting Client...");
        NetworkManager.Singleton.StartClient();
    }

    void StartServer()
    {
        Debug.Log("Starting Server...");
        NetworkManager.Singleton.StartServer();
    }
}
