using System.Collections.Generic;
using UnityEngine;

/* In Unity
 * X = left/right
 * Y = up/down
 * Z = forward/back
 */

namespace MoreVanillaBuildPrefabs.Helpers
{
    public class SnapPointHelper
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
        private static readonly List<Vector3> edgeMidPoints = new()
        {
            new Vector3(-1, -1, 0),
            new Vector3(0, -1, -1),
            new Vector3(0, -1, 1),
            new Vector3(1, -1, 0),
            new Vector3(-1, 0, 0),
            new Vector3(0, 0, -1),
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 0),
            new Vector3(-1, 1, 0),
            new Vector3(0, 1, -1),
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 0),
        };

        /// <summary>
        ///     Adds snap points for the game object to the corners of the specified mesh.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="meshName"></param>
        internal static void AddSnapPointsToMeshCorners(
            GameObject target,
            string meshName,
            bool fixPiece = false
        )
        {
            var mesh = target.GetMesh(meshName);

            if (mesh == null) return;

            List<Vector3> pts = new();
            var bounds = mesh.bounds;
            foreach (var corner in corners)
            {
                pts.Add(bounds.center + Vector3.Scale(corner, bounds.extents));
            }
            AddSnapPoints(target, pts, fixPiece);
        }

        /// <summary>
        ///     Adds a snap point to the local center of the game object's transform.
        /// </summary>
        /// <param name="target"></param>
        internal static void AddSnapPointToCenter(GameObject target)
        {
            AddSnapPoints(
                target,
                new Vector3[]
                {
                    new Vector3(0.0f, 0.0f, 0.0f),
                }
            );
        }

        internal static void AddSnapPoints(
            GameObject target,
            IEnumerable<Vector3> points,
            bool fixPiece = false,
            bool fixZClipping = false
        )
        {
            if (fixPiece)
            {
                FixPiece(target);
            }

            float z = 0f;

            foreach (Vector3 point in points)
            {
                Vector3 pos = point;

                if (fixZClipping)
                {
                    pos.z = z;
                    z += 0.0001f;
                }

                CreateSnapPoint(pos, target.transform);
            }
        }


        private static void CreateSnapPoint(Vector3 pos, Transform parent)
        {
            GameObject snappoint = new("_snappoint");
            snappoint.transform.parent = parent;
            snappoint.transform.localPosition = pos;
            snappoint.tag = "snappoint";
            snappoint.SetActive(false);
        }


        internal static void FixPiece(GameObject target)
        {
            foreach (Collider collider in target.GetComponentsInChildren<Collider>())
            {
                collider.gameObject.layer = LayerMask.NameToLayer("piece");
            }
        }
    }
}
