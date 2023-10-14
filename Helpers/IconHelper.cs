using System;

​​using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Jotunn.Managers;

using MoreVanillaBuildPrefabs.Logging;


namespace MoreVanillaBuildPrefabs.Helpers
{
    internal class IconHelper : MonoBehaviour
    {
        private static GameObject _gameObject;
        private static IconHelper _instance;

        public static IconHelper Instance { get { return _instance; } }

        /// <summary>
        ///     Create and add Icons for list of custom pieces.
        /// </summary>
        /// <param name="prefabs"></param>
        internal void GeneratePrefabIcons(IEnumerable<GameObject> prefabs)
        {

        }

        private IEnumerator StartRendering(IEnumerable<GameObject> prefabs)
        {
            yield return StartCoroutine(StartRendering(prefabs));
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

                yield return null;
                Sprite result = GenerateObjectIcon(gameObject);
                if (result == null)
                {
                    PickableItem.RandomItem[] randomItemPrefabs = piece.gameObject.GetComponent<PickableItem>()?.m_randomItemPrefabs;
                    if (randomItemPrefabs != null && randomItemPrefabs.Length > 0)
                    {
                        GameObject item = randomItemPrefabs[0].m_itemPrefab?.gameObject;
                        if (item != null)
                        {
                            yield return null;
                            result = GenerateObjectIcon(item);
                        }
                    }
                }
                piece.m_icon = result;
            }
        }

        private static Sprite GenerateObjectIcon(GameObject obj)
        {
            var cache = true;
#if DEBUG
            cache = false;
#endif
            var request = new RenderManager.RenderRequest(obj)
            {
                Rotation = RenderManager.IsometricRotation,
                UseCache = cache
            };
            return RenderManager.Instance.Render(request);
        }
    }
}
