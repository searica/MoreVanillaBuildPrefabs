using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jotunn.Managers;

namespace MoreVanillaBuildPrefabs
{
    internal class PrefabIcons : MonoBehaviour
    {
        private static bool Rendering = false;
        private static readonly GameObject _parent = new();
        private static PrefabIcons _instance;

        /// <summary>
        ///     The singleton instance of this manager.
        /// </summary>
        public static PrefabIcons Instance => _instance ??= _parent.AddComponent<PrefabIcons>() as PrefabIcons;

        /// <summary>
        ///     Hide .ctor to prevent other instances from being created
        /// </summary>
        private PrefabIcons() { }


        /// <summary>
        ///     Create and add Icons for list of prefabs with pieces.
        /// </summary>
        /// <param name="prefabs"></param>
        internal void GeneratePrefabIcons(IEnumerable<GameObject> prefabs)
        {
            var renderingCoroutine = StartCoroutine(GeneratePrefabIconsCoroutine(prefabs));
            //var renderWatcher = StartCoroutine(WhileRendering());
            //StopCoroutine(renderingCoroutine);
            //StopCoroutine(renderWatcher);
        }

        // Refs:
        //  - CreatureSpawner.m_creaturePrefab
        //  - PickableItem.m_randomItemPrefabs
        //  - PickableItem.RandomItem.m_itemPrefab
        private IEnumerator GeneratePrefabIconsCoroutine(IEnumerable<GameObject> prefabs)
        {
            Rendering = true;
            foreach (var prefab in prefabs)
            {
                if (prefab == null) { Log.LogInfo($"Null prefab found"); }

                yield return null;
                Sprite result = GenerateObjectIcon(prefab);
                if (result == null)
                {
                    PickableItem.RandomItem[] randomItemPrefabs = prefab.GetComponent<PickableItem>()?.m_randomItemPrefabs;
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
                var piece = prefab.GetComponent<Piece>();
                if (piece != null)
                {
                    piece.m_icon = result;
                }
            }
            Rendering = false;
            Log.LogInfo("Rendering Complete");
        }

        private IEnumerator WhileRendering()
        {
            while (Rendering)
            {
#if DEBUG
                Log.LogInfo("Rendering");
#endif
                yield return new WaitForSeconds(0.1f);
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
