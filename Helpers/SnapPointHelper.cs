// Ignore Spelling: MVBP

using MVBP.Extensions;
using MVBP.Models;
using System.Collections.Generic;
using UnityEngine;
using static MVBP.Helpers.SnapPointNames;

/* In Unity
 * X = left/right
 * Y = up/down
 * Z = forward/back
 */

namespace MVBP.Helpers
{
    internal static class SnapPointHelper
    {
        // List of points in a 2x2 box that would be the corners
        private static readonly List<Vector3> corners = new()
        {
            new Vector3(-1, -1, -1),
            new Vector3(-1, -1, 1),
            new Vector3(1, -1, 1),
            new Vector3(1, -1, -1),
            new Vector3(-1, 1, -1),
            new Vector3(-1, 1, 1),
            new Vector3(1, 1, 1),
            new Vector3(1, 1, -1),
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
            if (gameObject == null || boxCollider == null)
            {
                return;
            }

            List<Vector3> pts = new();
            Vector3 extents = boxCollider.size / 2;
            foreach (Vector3 corner in corners)
            {
                pts.Add(boxCollider.center + Vector3.Scale(corner, extents));
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
            Mesh mesh = gameObject.GetMesh(meshName);

            if (mesh == null)
            {
                return;
            }

            List<Vector3> pts = new();
            Bounds bounds = mesh.bounds;
            foreach (Vector3 corner in corners)
            {
                pts.Add(bounds.center + Vector3.Scale(corner, bounds.extents));
            }
            AddSnapPoints(gameObject, pts);
        }

        /// <summary>
        ///     Adds a snap point to the local center of the game object's transform.
        /// </summary>
        /// <param name="gameObject"></param>
        internal static void AddSnapPointToOrigin(GameObject gameObject)
        {
            CreateSnapPoint(Vector3.zero, gameObject, ORIGIN);
        }

        internal static void AddSnapPoints(
            GameObject gameObject,
            IEnumerable<Vector3> points
        )
        {
            int i = 1;

            foreach (Vector3 point in points)
            {
                Vector3 pos = point;
                CreateSnapPoint(pos, gameObject, $"{SNAPPOINT} {i++}");
            }
        }

        internal static void AddSnapPoints(
            GameObject gameObject,
            NamedSnapPoint[] namedSnapPoints
        )
        {
            foreach (NamedSnapPoint point in namedSnapPoints)
            {
                CreateSnapPoint(point.LocalPosition, gameObject, point.Name);
            }
        }

        private static void CreateSnapPoint(Vector3 pos, GameObject prefab, string snapPointName)
        {
            GameObject snappoint = new(snapPointName);
            snappoint.transform.parent = prefab.transform;
            snappoint.transform.localPosition = pos;
            snappoint.tag = "snappoint";
            snappoint.SetActive(false);
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
    }
}
