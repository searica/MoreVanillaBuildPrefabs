using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jotunn.Managers;
using MoreVanillaBuildPrefabs.Logging;
using Jotunn.Entities;
using System.Linq;

namespace MoreVanillaBuildPrefabs.Helpers
{
    internal class IconHelper : MonoBehaviour
    {
        private static GameObject _parent;
        private static IconHelper _instance;
        private static Coroutine _coroutine;

        /// <summary>
        ///     The singleton instance of this manager.
        /// </summary>
        public static IconHelper Instance => CreateInstance();

        private static IconHelper CreateInstance()
        {
            if (_parent == null)
            {
                _parent = new GameObject();
                GameObject.DontDestroyOnLoad(_parent);
            }
            if (_instance == null)
            {
                _instance = _parent.AddComponent<IconHelper>();
            }
            return _instance;
        }

        /// <summary>
        ///     Hide .ctor to prevent other instances from being created
        /// </summary>
        private IconHelper() { }

        /// <summary>
        ///     Create and add Icons for list of custom pieces.
        /// </summary>
        /// <param name="prefabs"></param>
        internal void GeneratePrefabIcons(IEnumerable<CustomPiece> customPieces)
        {
            foreach (var customPiece in customPieces)
            {
                if (customPiece == null) { Log.LogInfo($"Null custom piece found"); }

                Sprite result = GenerateObjectIcon(customPiece.PiecePrefab);
                if (result == null)
                {
                    PickableItem.RandomItem[] randomItemPrefabs = customPiece.PiecePrefab.GetComponent<PickableItem>()?.m_randomItemPrefabs;
                    if (randomItemPrefabs != null && randomItemPrefabs.Length > 0)
                    {
                        GameObject item = randomItemPrefabs[0].m_itemPrefab?.gameObject;
                        if (item != null)
                        {
                            result = GenerateObjectIcon(item);
                        }
                    }
                }
                var piece = customPiece.Piece;
                if (piece != null)
                {
                    piece.m_icon = result;
                }
            }
        }

        private Sprite GenerateObjectIcon(GameObject obj)
        {
            var request = new RenderManager.RenderRequest(obj)
            {
                Rotation = RenderManager.IsometricRotation,
                UseCache = true
            };
            return RenderManager.Instance.Render(request);
        }
    }
}
