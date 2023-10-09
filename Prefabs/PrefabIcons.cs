using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jotunn.Managers;

namespace MoreVanillaBuildPrefabs
{
    internal class PrefabIcons
    {
        // Refs:
        //  - CreatureSpawner.m_creaturePrefab
        //  - PickableItem.m_randomItemPrefabs
        //  - PickableItem.RandomItem.m_itemPrefab

        private static IEnumerator GeneratePrefabIcons(IEnumerable<GameObject> prefabs)
        {
            foreach (var prefab in prefabs)
            {
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
                prefab.GetComponent<Piece>().m_icon = result;
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
