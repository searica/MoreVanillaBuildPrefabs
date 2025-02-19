// Ignore Spelling: MVBP

using Jotunn.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logging;

namespace MVBP.PrefabManagement;

internal class IconManager : MonoBehaviour
{
    private static readonly HashSet<string> DoNotCacheIcon =
    [
        "portal",
        "dvergrprops_wood_floor",
        "dvergrprops_wood_stair",
    ];

    internal static bool ShouldCacheIcon(string name) => !DoNotCacheIcon.Contains(name);

    private static GameObject _gameObject;
    private static IconManager _instance;

    /// <summary>
    ///     The singleton instance of this manager.
    /// </summary>
    internal static IconManager Instance => CreateInstance();

    private static IconManager CreateInstance()
    {
        if (_gameObject == null)
        {
            _gameObject = new GameObject();
            DontDestroyOnLoad(_gameObject);
        }

        if (_instance == null)
        {
            _instance = _gameObject.AddComponent<IconManager>();
        }

        return _instance;
    }

    /// <summary>
    ///     Hide .ctor to prevent other instances from being created
    /// </summary>
    private IconManager() { }

    public void GeneratePrefabIcons(IEnumerable<GameObject> prefabs)
    {
        StartCoroutine(RenderCoroutine(prefabs));
    }

    private IEnumerator RenderCoroutine(IEnumerable<GameObject> gameObjects)
    {
        foreach (GameObject gameObject in gameObjects)
        {
            if (!gameObject)
            {
                Log.LogWarning($"Null prefab, cannot render icon");
                continue;
            }

            if (!gameObject.TryGetComponent(out Piece piece))
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
                PickableItem.RandomItem[] randomItemPrefabs = piece.GetComponent<PickableItem>()?.m_randomItemPrefabs;

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

        // update icons once they have all been rendered
        ModCompat.UpdatePlanBuild();
    }

    private static Sprite GenerateObjectIcon(GameObject obj)
    {
        RenderManager.RenderRequest request = new(obj)
        {
            Rotation = RenderManager.IsometricRotation,
            UseCache = ShouldCacheIcon(obj.name)
        };

        return RenderManager.Instance.Render(request);
    }
}
