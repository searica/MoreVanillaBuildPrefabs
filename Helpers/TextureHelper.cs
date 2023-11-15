using System.Drawing.Imaging;
using System.Drawing;
using System.IO;

using UnityEngine;
using Jotunn.Managers;

namespace MVBP.Helpers
{
    internal class TextureHelper
    {
        private const string armorStandTextureName = "Planks5c_low";
        private static Texture armorStandTexture;
        private static Material armorStandMaterial;

        private const string bmPortalTextureName = "texture_portal_MainTex.png";
        private const string bmPortalBumpMapName = "texture_portal_n_BumpMap.png";
        private static Texture2D bmPortalTexture;
        private static Texture2D bmPortalBumpMap;

        private const string cleanWoodTextureName = "spiralstair_d";
        private static Texture cleanWoodTexture;

        internal static class TextureNames
        {
            public const string SkinMap = "_SkinBumpMap";
            public const string BumpMap = "_BumpMap";
            public const string Main = "_MainTex";
        }

        internal static Material GetCustomArmorStandMaterial()
        {
            if (armorStandMaterial == null)
            {
                var playerBody = PrefabManager.Instance?.GetPrefab("Player")?.transform?.Find("Visual")?.Find("body");
                var playerMeshRender = playerBody?.GetComponent<SkinnedMeshRenderer>();
                var playerMaterial = playerMeshRender.sharedMaterial;
                armorStandMaterial = new Material(playerMaterial)
                {
                    name = "CustomArmorStand",
                    mainTexture = GetArmorStandTexture(),
                    shader = playerMaterial.shader,
                };
                if (armorStandMaterial == null)
                {
                    Log.LogWarning("Failed to get custom armor stand material");
                }
            }
            return armorStandMaterial;
        }

        internal static Texture GetArmorStandTexture()
        {
            if (armorStandTexture == null)
            {
                armorStandTexture = PrefabManager.Cache.GetPrefab<Texture>(armorStandTextureName);
                if (armorStandTexture == null)
                {
                    Log.LogWarning($"Failed to find {armorStandTextureName}");
                }
            }
            return armorStandTexture;
        }

        internal static Texture GetNewDvergrTexture()
        {
            if (cleanWoodTexture == null)
            {
                cleanWoodTexture = PrefabManager.Cache.GetPrefab<Texture>(cleanWoodTextureName);
                if (cleanWoodTexture == null)
                {
                    Log.LogWarning($"Failed to find {cleanWoodTextureName}");
                }
            }
            return cleanWoodTexture;
        }

        internal static Texture2D GetBlackMarblePortalTexture()
        {
            if (bmPortalTexture == null)
            {
                bmPortalTexture = LoadTextureFromResources(bmPortalTextureName);
            }
            return bmPortalTexture;
        }

        internal static Texture2D GetBlackMarblePortalBumpMap()
        {
            if (bmPortalBumpMap == null)
            {
                bmPortalBumpMap = LoadTextureFromResources(bmPortalBumpMapName);
            }
            return bmPortalBumpMap;
        }

        internal static Texture2D LoadTextureFromResources(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLower();
            if (extension != ".png" && extension != ".jpg")
            {
                Log.LogWarning("LoadTextureFromResources can only load png or jpg textures");
                return null;
            }
            fileName = Path.GetFileNameWithoutExtension(fileName);

            if (Properties.Resources.ResourceManager.GetObject(fileName) is not Bitmap resource)
            {
                Log.LogWarning($"Failed to find texture: {fileName + extension} in resources");
                return null;
            }

            var texture = new Texture2D(0, 0);
            using (var mStream = new MemoryStream())
            {
                switch (extension)
                {
                    case ".jpg":
                        resource.Save(mStream, ImageFormat.Jpeg);
                        break;

                    case ".png":
                        resource.Save(mStream, ImageFormat.Png);
                        break;
                }

                var buffer = new byte[mStream.Length];
                mStream.Position = 0;
                mStream.Read(buffer, 0, buffer.Length);
                texture.LoadImage(buffer);
            }
            return texture;
        }
    }
}