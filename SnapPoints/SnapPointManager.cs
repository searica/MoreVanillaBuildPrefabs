// Ignore Spelling: MVBP

using MVBP.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static MVBP.SnapPoints.SnapPointNames;
using static UnityEngine.UI.GridLayoutGroup;

/* In Unity
 * X = left/right
 * Y = up/down
 * Z = forward/back
 */

namespace MVBP.SnapPoints;

internal static class SnapPointManager
{

    // Map of snap point names to the corners of a 2x2 box centered on the origin
    private static readonly Dictionary<string, Vector3> Corners = new() {
        { $"{SnapPointNames.BOTTOM} {SnapPointNames.CORNER} 1", new Vector3(-1, -1, -1) },
        { $"{SnapPointNames.BOTTOM} {SnapPointNames.CORNER} 2", new Vector3(-1, -1, 1) },
        { $"{SnapPointNames.BOTTOM} {SnapPointNames.CORNER} 3", new Vector3(1, -1, 1) },
        { $"{SnapPointNames.BOTTOM} {SnapPointNames.CORNER} 4", new Vector3(1, -1, -1) },
        { $"{SnapPointNames.TOP} {SnapPointNames.CORNER} 1", new Vector3(-1, 1, -1) },
        { $"{SnapPointNames.TOP} {SnapPointNames.CORNER} 2", new Vector3(-1, 1, 1) },
        { $"{SnapPointNames.TOP} {SnapPointNames.CORNER} 3", new Vector3(1, 1, 1) },
        { $"{SnapPointNames.TOP} {SnapPointNames.CORNER} 4", new Vector3(1, 1, -1) },

    };

    // List of points in a 2x2 box that would be the middle of each edge
    //private static readonly List<Vector3> edgeMidPoints = new()
    //{
    //    new Vector3(-1, -1, 0),
    //    new Vector3(0, -1, -1),
    //    new Vector3(0, -1, 1),
    //    new Vector3(1, -1, 0),
    //    new Vector3(-1, 0, 0),
    //    new Vector3(0, 0, -1),
    //    new Vector3(0, 0, 1),
    //    new Vector3(1, 0, 0),
    //    new Vector3(-1, 1, 0),
    //    new Vector3(0, 1, -1),
    //    new Vector3(0, 1, 1),
    //    new Vector3(1, 1, 0),
    //};

    internal static void AddSnapPoints(
        GameObject gameObject,
        IEnumerable<Vector3> points,
        string prefix = EXTRA, 
        int startNumber = 1
    )
    {
        AddSnapPoints(gameObject, CreateNamedSnapPoints(points, prefix, startNumber));
    }

    internal static void AddSnapPoints(
        GameObject gameObject,
        NamedSnapPoint[] namedSnapPoints
    )
    {
        foreach (NamedSnapPoint point in namedSnapPoints)
        {
            AddSnapPointInternal(point.LocalPosition, gameObject, point.Name);
        }
    }

    internal static void AddSnapPoints(
        GameObject gameObject,
        IEnumerable<NamedSnapPoint> namedSnapPoints
    )
    {
        foreach (NamedSnapPoint point in namedSnapPoints)
        {
            AddSnapPointInternal(point.LocalPosition, gameObject, point.Name);
        }
    }

    /// <summary>
    ///     Adds a snap point to the local center of the game object's transform.
    /// </summary>
    /// <param name="gameObject"></param>
    internal static void AddSnapPointToOrigin(GameObject gameObject)
    {
        AddSnapPointInternal(Vector3.zero, gameObject, ORIGIN);
    }

    /// <summary>
    ///     Adds snap points for the game object to the corners of the specified mesh.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="boxCollider"></param>
    /// <param name="fixPiece"></param>
    internal static void AddSnapPointsToBoxColliderCorners(
        GameObject gameObject,
        BoxCollider boxCollider
    )
    {
        if (!gameObject || boxCollider == null)
        {
            return;
        }

        List<NamedSnapPoint> pts = [];
        Vector3 extents = boxCollider.size / 2;
        foreach (KeyValuePair<string, Vector3> kvp in Corners)
        {
            pts.Add(new NamedSnapPoint(boxCollider.center + Vector3.Scale(kvp.Value, extents), kvp.Key));
        }
        AddSnapPoints(gameObject, pts);
    }

    /// <summary>
    ///     Adds snap points for the game object to the corners of the specified mesh.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="meshName"></param>
    /// <param name="fixPiece"></param>
    internal static void AddSnapPointsToMeshCorners(
        GameObject gameObject,
        string meshName
    )
    {
        if (!gameObject.TryGetMesh(meshName, out Mesh mesh))
        {
            return;
        }

        List<NamedSnapPoint> pts = [];
        Bounds bounds = mesh.bounds;
        foreach (KeyValuePair<string, Vector3> kvp in Corners)
        {
            pts.Add(new NamedSnapPoint(bounds.center + Vector3.Scale(kvp.Value, bounds.extents), kvp.Key));
        }
        AddSnapPoints(gameObject, pts);
    }

    /// <summary>
    ///     Set all colliders to the piece layer.
    /// </summary>
    /// <param name="gameObject"></param>
    internal static void FixPieceLayers(GameObject gameObject)
    {
        foreach (Collider collider in gameObject.GetComponentsInChildren<Collider>())
        {
            collider.gameObject.layer = LayerMask.NameToLayer("piece");
        }
    }

    private static void AddSnapPointInternal(Vector3 pos, GameObject prefab, string snapPointName)
    {
        GameObject snappoint = new(snapPointName);
        snappoint.transform.parent = prefab.transform;
        snappoint.transform.localPosition = pos;
        snappoint.tag = SnapPointNames.TAG;
        snappoint.SetActive(false);
    }

    private static NamedSnapPoint[] CreateNamedSnapPoints(Vector3[] positions, string prefix = EXTRA, int startNumber = 1)
    {
        NamedSnapPoint[] result = new NamedSnapPoint[positions.Length];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = new NamedSnapPoint(positions[i], $"{prefix} {startNumber + i}");
        }
        return result;
    }

    private static NamedSnapPoint[] CreateNamedSnapPoints(IEnumerable<Vector3> positions, string prefix = EXTRA, int startNumber = 1)
    {
        NamedSnapPoint[] result = new NamedSnapPoint[positions.Count()];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = new NamedSnapPoint(positions.ElementAt(i), $"{prefix} {startNumber + i}");
        }
        return result;
    }
}
