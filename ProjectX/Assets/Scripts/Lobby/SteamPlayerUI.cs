using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Steamworks;

public class SteamPlayerUI : MonoBehaviour
{
    [Header("UI References")]
    public RawImage avatarImage;
    public TextMeshProUGUI nameText;

    private CSteamID steamID;

    /// <summary>
    /// Call this after instantiating the prefab to assign the Steam player it represents.
    /// </summary>
    public void Init(CSteamID id)
    {
        steamID = id;

        if (!SteamManager.Initialized)
        {
            Debug.LogWarning("Steam not initialized");
            return;
        }

        // Set name
        nameText.text = SteamFriends.GetFriendPersonaName(steamID);

        // Set avatar
        int avatarInt = SteamFriends.GetLargeFriendAvatar(steamID);
        if (avatarInt != -1)
        {
            Texture2D avatarTex = GetSteamImageAsTexture(avatarInt);
            if (avatarTex != null)
            {
                avatarImage.texture = avatarTex;
            }
        }
        else
        {
            Debug.Log("Avatar not yet ready for " + steamID);
        }
    }

    private Texture2D GetSteamImageAsTexture(int imageID)
    {
        if (!SteamUtils.GetImageSize(imageID, out uint width, out uint height))
            return null;

        byte[] imageData = new byte[width * height * 4];

        if (SteamUtils.GetImageRGBA(imageID, imageData, imageData.Length))
        {
            Texture2D texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
            texture.LoadRawTextureData(imageData);
            texture.Apply();
            return texture;
        }

        return null;
    }
}
