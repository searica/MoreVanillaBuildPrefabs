using System;
using UnityEngine;

namespace MVBP.Extensions;

internal static class GameObjectExtensions
{

    private const string MineRock5Name = "___MineRock5";
    /// <summary>
    ///     Gets the root Prefab name using Utils.GetPrefabName or using the MineRock5Tracker component if that is present.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    internal static string GetPrefabName(this GameObject gameObject)
    {
        if (gameObject.name.Contains(MineRock5Name) && gameObject.TryGetComponent(out MineRock5Extensions.MineRock5Tracker tracker))
        {
            return tracker.m_prefabName;
        }
        return Utils.GetPrefabName(gameObject);
    }

    /// <summary>
    ///     Creates a deep copy of the object.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    internal static GameObject DeepCopy(this GameObject obj)
    {
        // set the object to be inactive to avoid Null Ref Exceptions
        bool setActive = obj.activeSelf;
        obj.SetActive(false);

        GameObject clone = UnityEngine.Object.Instantiate(obj);

        // set object and clone to original state
        obj.SetActive(setActive);
        return clone;
    }

    /// <summary>
    ///     Check if GameObject has any of the specified components.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="components"></param>
    /// <returns></returns>
    public static bool HasAnyComponent(this GameObject gameObject, params Type[] components)
    {
        foreach (Type compo in components)
        {
            if (gameObject.GetComponent(compo))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     Check if GameObject has any of the specified components.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="componentNames"></param>
    /// <returns></returns>
    public static bool HasAnyComponent(this GameObject gameObject, params string[] componentNames)
    {
        foreach (string name in componentNames)
        {
            if (gameObject.GetComponent(name))
            {
                return true;
            }
        }
        return false;
    }


    /// <summary>
    ///     Extension method to find nested children by name using either
    ///     a breadth-first or depth-first search. Default is breadth-first.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="childName">Name of the child object to search for.</param>
    /// <param name="searchType">Whether to preform a breadth first or depth first search. Default is breadth first.</param>
    public static Transform FindDeepChild(
        this GameObject gameObject,
        string childName,
        global::Utils.IterativeSearchType searchType = global::Utils.IterativeSearchType.BreadthFirst
    )
    {
        return gameObject.transform.FindDeepChild(childName, searchType);
    }


    internal static Mesh GetMesh(this GameObject gameObject, string meshName)
    {
        foreach (MeshFilter meshFilter in gameObject.GetComponentsInChildren<MeshFilter>())
        {
            Mesh mesh = meshFilter.mesh;
            if (mesh == null)
            {
                continue;
            }

            if (mesh.name.RemoveSuffix("Instance").Trim() == meshName)
            {
                return mesh;
            }
        }

        Log.LogWarning($"Could not find Mesh: {meshName} for GameObject: {gameObject.name}");
        return null;
    }
}
