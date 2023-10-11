using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jotunn.Managers;
using MoreVanillaBuildPrefabs.Logging;

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
        ///     Create and add Icons for list of prefabs with pieces.
        /// </summary>
        /// <param name="prefabs"></param>
        internal void StartGeneratePrefabIcons(IEnumerable<GameObject> prefabs)
        {
            GeneratePrefabIconsCoroutine(prefabs);
            //_coroutine = StartCoroutine(GeneratePrefabIconsCoroutine(prefabs));
        }

        //internal void StopGeneratePrefabIcons()
        //{
        //    if (_coroutine != null)
        //    {
        //        StopCoroutine(_coroutine);
        //    }
        //}

        // Refs:
        //  - CreatureSpawner.m_creaturePrefab
        //  - PickableItem.m_randomItemPrefabs
        //  - PickableItem.RandomItem.m_itemPrefab
        private void GeneratePrefabIconsCoroutine(IEnumerable<GameObject> prefabs)
        {
            foreach (var prefab in prefabs)
            {
                if (prefab == null) { Log.LogInfo($"Null prefab found"); }

                //yield return null;
                Sprite result = GenerateObjectIcon(prefab);
                if (result == null)
                {
                    PickableItem.RandomItem[] randomItemPrefabs = prefab.GetComponent<PickableItem>()?.m_randomItemPrefabs;
                    if (randomItemPrefabs != null && randomItemPrefabs.Length > 0)
                    {
                        GameObject item = randomItemPrefabs[0].m_itemPrefab?.gameObject;
                        if (item != null)
                        {
                            //yield return null;
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
            //yield return null;
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
