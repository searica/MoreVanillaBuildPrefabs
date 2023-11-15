// Ignore Spelling: MVBP

using Jotunn.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MVBP.Configs;

namespace MVBP.Helpers
{
    internal class IconHelper : MonoBehaviour
    {
        private static GameObject _gameObject;
        private static IconHelper _instance;

        /// <summary>
        ///     The singleton instance of this manager.
        /// </summary>
        internal static IconHelper Instance => CreateInstance();

        private static IconHelper CreateInstance()
        {
            if (_gameObject == null)
            {
                _gameObject = new GameObject();
                DontDestroyOnLoad(_gameObject);
            }
            if (_instance == null)
            {
                _instance = _gameObject.AddComponent<IconHelper>();
            }
            return _instance;
        }

        /// <summary>
        ///     Hide .ctor to prevent other instances from being created
        /// </summary>
        private IconHelper()
        { }

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