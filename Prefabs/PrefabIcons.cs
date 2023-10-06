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
        public static Sprite CreatePrefabIcon(GameObject prefab)
        {
            Sprite result = GenerateObjectIcon(prefab);

            if (result == null)
            {
                PickableItem.RandomItem[] randomItemPrefabs = prefab.GetComponent<PickableItem>()?.m_randomItemPrefabs;
                if (randomItemPrefabs != null && randomItemPrefabs.Length > 0)
                {
                    GameObject item = randomItemPrefabs[0].m_itemPrefab?.gameObject;
                    if (item != null)
                    {
                        result = GenerateObjectIcon(item);
                    }
                }
            }

            return result;
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
