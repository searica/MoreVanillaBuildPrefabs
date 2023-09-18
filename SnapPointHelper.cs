using MoreVanillaBuildPrefabs;
using System.Collections.Generic;
using UnityEngine;

/* In Unity
 * X = left/right
 * Y = up/down
 * Z = forward/back
 */

namespace MoreVanillaBuildPrefabs
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
            GameObject snappoint = new GameObject("_snappoint");
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

    // private class ExtraMethods
    // {
    //     public static void AddCenterSnapPoint(string name)
    //     {
    //         /*
    //         Add a snap points to the relative center of the piece.
    //         */
    //         AddSnapPoints(name, new[] new Vector3(0.0f, 0.0f, 0.0f));
    //     }


    //     public static void AddCornerSnapPoints(
    //         string name, float width, float height, float depth = 0.0f
    //     )
    //     {
    //         /*
    //         Adds snap points to the corners of a rectangular prism
    //         or 2d rectangle defined by the provided dimensions.
    //         */
    //         corners = CalculateCorners(width, height, depth);
    //         AddSnapPoints(name, corners)
    //     }


    //     public static void AddMidEdgeSnapPoints(
    //         string name, float width, float height, float depth = 0.0f
    //     )
    //     {
    //         /*
    //         Adds snap points to the middle of each edge of a rectangular prism
    //         or 2d rectangle defined by the provided dimensions.
    //         */
    //         mid_edge_pts = CalculateMidEdgePoints(width, height, depth);
    //         AddSnapPoints(name, mid_edge_pts)
    //     }


    //     public static void AddMidFaceSnapPoints(
    //         string name, float width, float height, float depth
    //     )
    //     {
    //         /*
    //         Adds snap points to the middle of each face of a rectangular prism
    //         defined by the provided dimensions.
    //         */
    //         mid_face_pts = CalculateMidFacePoints(width, height, depth);
    //         AddSnapPoints(name, mid_face_pts)
    //     }


    //     static List<Vector3> CalculateCorners(
    //         float width, float height, float depth = 0
    //     )
    //     {
    //         /*
    //         If depth is 0 then computes the 4 corners of a flat shape as
    //         positions relative to the center of the objects given the width
    //         and height.

    //         If depth is non-zero then it calculates the 8 corners of the
    //         shape relative to the center using the provided dimensions.

    //         Center is assumed to be with respect to the provided dimensions.
    //         */
    //         List<Vector3> corners = new List<Vector3>();

    //         int[] nums = new int[] {-1, 1};

    //         foreach (var i in nums){
    //             foreach (var j in nums){
    //                 if (IsApproxZero(depth)){
    //                     corners.Add(
    //                         new Vector3(){i*width/2, j*height/2, 0.0f}
    //                     );
    //                 }
    //                 else {
    //                     foreach (var k in nums) {
    //                         corners.Add(
    //                             new Vector3(){i*width/2, j*height/2, k*depth/2}
    //                         );

    //                     }
    //                 }
    //             }
    //         }
    //         return corners
    //     }


    //     public static List<Vector3> CalculateMidEdgePoints(
    //         float width, float height, float depth = 0.0f
    //     )
    //     {
    //         /*
    //         If depth is 0 then computes the midpoint for each of the 4 edges
    //         of a flat shape as positions relative to the center of the objects given the width and height.

    //         If depth is non-zero then it calculates the midpoints for each of the 12 edges of the shape shape relative to the center using the provided dimensions.

    //         Center is assumed to be with respect to the provided dimensions.

    //         Midpoints are the "x" points on the ASCII shape below.
    //         ----x----
    //         |       |
    //         x       x
    //         |       |
    //         ----x----
    //         */
    //         Vector3[] mid_pos = new Vector3[] {
    //             new Vector3(-0.5f, -0.5f, 0.0f),
    //             new Vector3(-0.5f, 0.5f, 0.0f),
    //             new Vector3(0.5f, -0.5f, 0.0f),
    //             new Vector3(0.5f, 0.5f, 0.0f),
    //             new Vector3(-0.5f, 0.0f, -0.5f),
    //             new Vector3(-0.5f, 0.0f, 0.5f),
    //             new Vector3(0.5f, 0.0f, -0.5f),
    //             new Vector3(0.5f, 0.0f, 0.5f),
    //             new Vector3(0.0f, 0.5f, -0.5f),
    //             new Vector3(0.0f, 0.5f, 0.5f),
    //             new Vector3(0.0f, -0.5f, -0.5f),
    //             new Vector3(0.0f, -0.5f, 0.5f),
    //         }

    //         List<Vector3> midpoints = new List<Vector3>();

    //         if (IsApproxZero(depth)){
    //             // use range to get the first 4 elements of the mid_pos array
    //             foreach (var pos in mid_pos[..5]){
    //                 midpoints.Add(
    //                     new Vector3(){i*width, j*height, 0.0f});
    //             }
    //         }
    //         else {
    //             foreach (var pos in mid_pos){
    //                 midpoints.Add(
    //                     new Vector3(){i*width, j*height, k*depth});
    //             }
    //         }
    //         return midpoints
    //     }


    //     public static List<Vector3> CalculateMidFacePoints(
    //         float width, float height, float depth
    //     )
    //     {
    //         /*
    //         Computes the midpoint of each face of a rectangular prism with the
    //         provided dimensions.

    //         Points are defined relative to the center of the object.
    //         */
    //         Vector3[] mid_pos = new Vector3[] {
    //             new Vector3(-0.5f, 0.0f, 0.0f),
    //             new Vector3(0.5f, 0.0f, 0.0f),
    //             new Vector3(0.0f, -0.5f, 0.0f),
    //             new Vector3(0.0f, 0.5f, 0.0f),
    //             new Vector3(0.0f, 0.0f, -0.5f),
    //             new Vector3(0.0f, 0.0f, 0.5f),
    //         }

    //         List<Vector3> midpoints = new List<Vector3>();

    //         foreach (var pos in mid_pos){
    //             midpoints.Add(
    //                 new Vector3(){i*width, j*height, k*depth});
    //         }

    //         return midpoints
    //     }


    //     private static bool IsApproxZero(float val){
    //         double eps = 0.000001
    //         if (Math.Abs(val) <= eps){
    //             return true;
    //         }
    //         return false;
    //     }
    // }
}
