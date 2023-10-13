using System.Collections.Generic;
using UnityEngine;
using Jotunn.Managers;
using MoreVanillaBuildPrefabs.Logging;


namespace MoreVanillaBuildPrefabs.Helpers
{
    internal class IconHelper
    {
        /// <summary>
        ///     Create and add Icons for list of custom pieces.
        /// </summary>
        /// <param name="prefabs"></param>
        internal static void GeneratePrefabIcons(IEnumerable<Piece> pieces)
        {
            foreach (var piece in pieces)
            {
                if (piece == null)
                {
                    Log.LogInfo($"Null custom piece found");
                    continue;
                }

                Sprite result = GenerateObjectIcon(piece.gameObject);
                if (result == null)
                {
                    PickableItem.RandomItem[] randomItemPrefabs = piece.gameObject.GetComponent<PickableItem>()?.m_randomItemPrefabs;
                    if (randomItemPrefabs != null && randomItemPrefabs.Length > 0)
                    {
                        GameObject item = randomItemPrefabs[0].m_itemPrefab?.gameObject;
                        if (item != null)
                        {
                            result = GenerateObjectIcon(item);
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
                UseCache = true
            };
            return RenderManager.Instance.Render(request);
        }
    }
}
