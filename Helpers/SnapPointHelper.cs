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
        public static void AddCenterSnapPoint(GameObject target)
        {
            AddSnapPoints(
                target,
                new Vector3[]
                {
                    new Vector3(0.0f, 0.0f, 0.0f),
                }
            );
        }

        public static void AddSnapPoints(
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


        public static void FixPiece(GameObject target)
        {
            foreach (Collider collider in target.GetComponentsInChildren<Collider>())
            {
                collider.gameObject.layer = LayerMask.NameToLayer("piece");
            }
        }
    }
}
