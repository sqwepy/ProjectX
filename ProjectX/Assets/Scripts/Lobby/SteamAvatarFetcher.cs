using System.Collections;
using UnityEngine;
using Steamworks;

//public static class SteamAvatarFetcher
//{
//    public static IEnumerator SetAvatarImage(RawImage image, CSteamID steamID)
//    {
//        int avatarInt = SteamFriends.GetLargeFriendAvatar(steamID);
//        while (avatarInt == -1)
//        {
//            yield return null; // wait until avatar is loaded
//        }
//
//        uint width, height;
//        if (SteamUtils.GetImageSize(avatarInt, out width, out height))
//        {
//            byte[] imageBuffer = new byte[4 * (int)width * (int)height];
//            if (SteamUtils.GetImageRGBA(avatarInt, imageBuffer, imageBuffer.Length))
//            {
//                Texture2D avatarTexture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
//                avatarTexture.LoadRawTextureData(imageBuffer);
//                avatarTexture.Apply();
//
//                image.texture = avatarTexture;
//            }
//        }
//    }
//}