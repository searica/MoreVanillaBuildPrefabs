// Ignore Spelling: MVBP

using Jotunn.Managers;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using UnityEngine;
using MVBP.Configs;

namespace MVBP.Helpers
{
    internal class ImgHelper : MonoBehaviour
    {
        private static GameObject _gameObject;
        private static ImgHelper _instance;
        private static Texture cleanTexture;

        internal static Texture GetNewDvergrTexture()
        {
            if (cleanTexture == null)
            {
                GameObject template = ZNetScene.instance?.GetPrefab("piece_dvergr_spiralstair");
                cleanTexture = template?.transform?.Find("New")?.GetComponentInChildren<Renderer>()?.material?.mainTexture;
            }
            return cleanTexture;
        }

        /// <summary>
        ///     The singleton instance of this manager.
        /// </summary>
        internal static ImgHelper Instance => CreateInstance();

        private static ImgHelper CreateInstance()
        {
            if (_gameObject == null)
            {
                _gameObject = new GameObject();
                DontDestroyOnLoad(_gameObject);
            }
            if (_instance == null)
            {
                _instance = _gameObject.AddComponent<ImgHelper>();
            }
            return _instance;
        }

        /// <summary>
        ///     Hide .ctor to prevent other instances from being created
        /// </summary>
        private ImgHelper()
        { }

        internal static Texture2D LoadTextureFromResources(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLower();
            if (extension != ".png" && extension != ".jpg")
            {
                Log.LogWarning("LoadTextureFromResources can only load png or jpg textures");
                return null;
            }
            fileName = Path.GetFileNameWithoutExtension(fileName);

            var resource = Properties.Resources.ResourceManager.GetObject(fileName) as Bitmap;
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
                var texture = new Texture2D(0, 0);
                texture.LoadImage(buffer);
                return texture;
            }
        }

        public void GeneratePrefabIcons(IEnumerable<GameObject> prefabs)
        {
            StartCoroutine(RenderCoroutine(prefabs));
        }

        private IEnumerator RenderCoroutine(IEnumerable<GameObject> gameObjects)
        {
            foreach (var gameObject in gameObjects)
            {
                if (gameObject == null)
                {
                    Log.LogWarning($"Null prefab, cannot render icon");
                    continue;
                }

                var piece = gameObject.GetComponent<Piece>();

                if (piece == null)
                {
                    Log.LogWarning($"Null piece, cannot render icon");
                    continue;
                }

                Sprite result = GenerateObjectIcon(gameObject);
                // returning WaitForEndOfFrame seems to
                // fix the lighting bug in the icons
                yield return new WaitForEndOfFrame();

                if (result == null)
                {
                    PickableItem.RandomItem[] randomItemPrefabs = piece.gameObject.GetComponent<PickableItem>()
                        ?.m_randomItemPrefabs;

                    if (randomItemPrefabs != null && randomItemPrefabs.Length > 0)
                    {
                        GameObject item = randomItemPrefabs[0].m_itemPrefab?.gameObject;
                        if (item != null)
                        {
                            result = GenerateObjectIcon(item);
                            // returning WaitForEndOfFrame seems to
                            // fix the lighting bug in the icons
                            yield return new WaitForEndOfFrame();
                        }
                    }
                }

                piece.m_icon = result;
            }
        }

        private static Sprite GenerateObjectIcon(GameObject obj)
        {
            var request = new RenderManager.RenderRequest(obj)
            {
                Rotation = RenderManager.IsometricRotation,
                UseCache = PrefabConfigs.ShouldCacheIcon(obj.name)
            };
            return RenderManager.Instance.Render(request);
        }
    }
}