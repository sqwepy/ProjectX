using UnityEngine;
using Steamworks;

public class SteamManager : MonoBehaviour
{
    private static SteamManager s_instance;
    private static bool s_EverInitialized;

    private bool m_bInitialized;
    public static bool Initialized => s_instance != null && s_instance.m_bInitialized;

    private void Awake()
    {
        if (s_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        s_instance = this;
        DontDestroyOnLoad(gameObject);

        try
        {
            if (!Packsize.Test())
            {
                Debug.LogError("Packsize Test failed.");
            }

            if (!DllCheck.Test())
            {
                Debug.LogError("DllCheck Test failed.");
            }

            m_bInitialized = SteamAPI.Init();
            if (!m_bInitialized)
            {
                Debug.LogError("SteamAPI_Init() failed.");
            }
        }
        catch (System.DllNotFoundException e)
        {
            Debug.LogError("[Steamworks.NET] Could not load Steamworks DLL.");
            Debug.LogException(e);
        }
    }

    private void OnEnable()
    {
        if (s_instance == null)
            s_instance = this;
    }

    private void OnDestroy()
    {
        if (s_instance != this)
            return;

        s_instance = null;

        if (m_bInitialized)
        {
            SteamAPI.Shutdown();
        }
    }

    private void Update()
    {
        if (m_bInitialized)
        {
            SteamAPI.RunCallbacks();
        }
    }
}
