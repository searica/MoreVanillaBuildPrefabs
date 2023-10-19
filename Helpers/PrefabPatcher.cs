using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MoreVanillaBuildPrefabs.Logging;

namespace MoreVanillaBuildPrefabs.Helpers
{
    internal class PrefabPatcher
    {
        /// <summary>
        ///     Fix collider and snap points on the prefab if necessary
        /// </summary>
        /// <param name="prefab"></param>
        internal static void PatchPrefabIfNeeded(GameObject prefab)
        {
            if (prefab == null)
            {
                return;
            }

            List<Vector3> pts = new();
            switch (prefab.name)
            {
                case "ArmorStand_Male":
                case "ArmorStand_Female":
                    SnapPointHelper.AddSnapPointToCenter(prefab);
                    break;

                case "Trailership":
                    // Fix hull
                    var meshFilter = prefab.GetComponentInChildren<MeshFilter>("hull");
                    if (meshFilter == null) { break; }

                    var longShip = ZNetScene.instance?.m_prefabs?.Where(go => go.name == "VikingShip")?.First();
                    var longShipMeshFilter = longShip?.GetComponentInChildren<MeshFilter>("hull");
                    if (longShipMeshFilter == null) { break; }

                    meshFilter.mesh = longShipMeshFilter.mesh;

                    // Fix shields
                    var shieldBanded = ZNetScene.instance?.m_prefabs?.Where(go => go.name == "ShieldBanded")?.First();
                    var mat = shieldBanded?.GetComponentInChildren<MeshRenderer>()?.material;
                    if (mat == null) { break; }

                    var storage = prefab.transform.Find("ship")
                        ?.Find("visual")
                        ?.Find("Customize")
                        ?.Find("storage");
                    if (storage == null) { break; }

                    int children = storage.childCount;
                    for (int i = 0; i < children; ++i)
                    {
                        var child = storage.GetChild(i);
                        if (child != null && child.name.StartsWith("Shield"))
                        {
                            child.GetComponent<MeshRenderer>().material = mat;
                        }
                    }

                    // fix sail
                    //var sailFull = prefab?.transform?.Find("ship")
                    //    ?.transform?.Find("visual")
                    //    ?.transform?.Find("Mast")
                    //    ?.transform?.Find("Sail")
                    //    ?.transform?.Find("sail_full").gameObject;
                    //var skinRender = sailFull?.GetComponent<SkinnedMeshRenderer>();
                    //if (skinRender == null) { break; }

                    //var localBounds = skinRender.localBounds;
                    //localBounds.center = new Vector3(0f, 0.4849242f, 0f);
                    //localBounds.extents = new Vector3(1.065068e-06f, 0.289977f, 4.467221f);
                    //skinRender.localBounds = localBounds;
                    //Log.LogInfo(skinRender.localBounds.center);
                    //Log.LogInfo(skinRender.localBounds.extents);

                    // Fix sail skinned mesh renderer
                    //var longShipSailFull = longShip?.transform?.Find("ship")
                    //    ?.transform?.Find("visual")
                    //    ?.transform?.Find("Mast")
                    //    ?.transform?.Find("Sail")
                    //    ?.transform?.Find("sail_full")?.gameObject;
                    //if (longShipSailFull == null) { break; }

                    //var sailFull = prefab?.transform?.Find("ship")
                    //    ?.transform?.Find("visual")
                    //    ?.transform?.Find("Mast")
                    //    ?.transform?.Find("Sail")
                    //    ?.transform?.Find("sail_full").gameObject;
                    //if (sailFull == null) { break; }
                    //var skinMeshRender = sailFull.GetComponent<SkinnedMeshRenderer>();
                    //skinMeshRender.bounds.extents.Scale(new Vector3(1, 1.1f, 1));

                    // Fix missing control GUI position
                    var ship = prefab.GetComponent<Ship>();
                    if (ship == null) { break; }
                    var controlGui = new GameObject("ControlGui");
                    controlGui.transform.parent = prefab.transform;
                    controlGui.transform.localPosition = new Vector3(1.0f, 1.696f, -6.54f);
                    ship.m_controlGuiPos = controlGui.transform;

                    // Fix missing rudder button attachpoint
                    var shipControls = prefab.transform?.Find("ship")
                        ?.Find("buttons")
                        ?.Find("rudder (1)")
                        ?.Find("rudder_button")
                        ?.GetComponent<ShipControlls>();
                    if (shipControls == null) { break; }

                    var rudderAttach = prefab?.transform?.Find("sit locations")
                        ?.Find("sit_box (4)")
                        ?.Find("attachpoint");
                    if (rudderAttach == null) { break; }

                    shipControls.m_attachPoint = rudderAttach.transform;

                    // Fix sail not catching wind
                    ship.m_sailForceOffset = 2; // same as longship
                    ship.m_sailForceFactor = 0.05f; // same as longship
                    break;

                case "TreasureChest_dvergr_loose_stone":
                    var boxCollider = prefab.AddComponent<BoxCollider>();
                    boxCollider.size = new Vector3(2, 1, 2);
                    SnapPointHelper.AddSnapPointsToBoxColliderCorners(prefab, boxCollider);
                    break;

                case "TreasureChest_mountaincave":
                case "TreasureChest_trollcave":
                    SnapPointHelper.AddSnapPointsToMeshCorners(prefab, "stonechest", true);
                    break;
                //case "TreasureChest_dvergrtown":
                //    break;
                //case "TreasureChest_dvergrtower":
                //    break;
                case "TreasureChest_plains_stone":
                case "TreasureChest_fCrypt":
                case "TreasureChest_sunkencrypt":
                    SnapPointHelper.AddSnapPoints(
                       prefab,
                       new[] {
                            new Vector3(0.0f, -0.01f, 0.0f),
                            new Vector3(1.0f, -0.01f, 0.37f),
                            new Vector3(1.0f, -0.01f, -0.37f),
                            new Vector3(-1.0f, -0.01f, 0.37f),
                            new Vector3(-1.0f, -0.01f, -0.37f),
                            new Vector3(0.65f, 0.8f, 0.35f),
                            new Vector3(0.65f, 0.8f, -0.35f),
                            new Vector3(-0.65f, 0.8f, 0.35f),
                            new Vector3(-0.65f, 0.8f, -0.35f)
                       },
                       true
                    );
                    break;

                case "blackmarble_column_3":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(-1, 4, -1),
                            new Vector3(1, 4, 1),
                            new Vector3(-1, 4, 1),
                            new Vector3(1, 4, -1),

                            new Vector3(-1, 2, -1),
                            new Vector3(1, 2, 1),
                            new Vector3(-1, 2, 1),
                            new Vector3(1, 2, -1),

                            new Vector3(-1, 0, -1),
                            new Vector3(1, 0, 1),
                            new Vector3(-1, 0, 1),
                            new Vector3(1, 0, -1),

                            new Vector3(-1, -2, -1),
                            new Vector3(1, -2, 1),
                            new Vector3(-1, -2, 1),
                            new Vector3(1, -2, -1),

                            new Vector3(-1, -4, -1),
                            new Vector3(1, -4, 1),
                            new Vector3(-1, -4, 1),
                            new Vector3(1, -4, -1),
                        }
                    );
                    break;

                case "blackmarble_creep_4x1x1":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(-0.5f, 2, -0.5f),
                            new Vector3(0.5f, 2, 0.5f),
                            new Vector3(-0.5f, 2, 0.5f),
                            new Vector3(0.5f, 2, -0.5f),

                            new Vector3(-0.5f, -2, -0.5f),
                            new Vector3(0.5f, -2, 0.5f),
                            new Vector3(-0.5f, -2, 0.5f),
                            new Vector3(0.5f, -2, -0.5f),

                            new Vector3(0, -2, 0),
                            new Vector3(0, 2, 0),
                        }
                    );

                    // ? Place the piece randomly in horizontal or vertical position ?
                    UnityEngine.Object.DestroyImmediate(prefab.transform.Find("new").gameObject.GetComponent<RandomPieceRotation>());

                    break;

                case "blackmarble_creep_4x2x1":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(1, 2, -0.5f),
                            new Vector3(1, 2, 0.5f),
                            new Vector3(-1, 2, -0.5f),
                            new Vector3(-1, 2, 0.5f),

                            new Vector3(1, -2, -0.5f),
                            new Vector3(1, -2, 0.5f),
                            new Vector3(-1, -2, -0.5f),
                            new Vector3(-1, -2, 0.5f),

                            new Vector3(0, -2, 0),
                            new Vector3(0, 2, 0),
                        }
                    );

                    // ? Place the piece randomly in horizontal or vertical position ?
                    UnityEngine.Object.DestroyImmediate(prefab.transform.Find("new").gameObject.GetComponent<RandomPieceRotation>());
                    break;

                case "blackmarble_creep_slope_inverted_1x1x2":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(-0.5f, 1, -0.5f),
                            new Vector3(0.5f, 1, 0.5f),
                            new Vector3(-0.5f, 1, 0.5f),
                            new Vector3(0.5f, 1, -0.5f),

                            new Vector3(0.5f, -1, -0.5f),
                            new Vector3(-0.5f, -1, -0.5f),
                        }
                    );
                    break;

                case "blackmarble_creep_slope_inverted_2x2x1":
                    SnapPointHelper.AddSnapPoints(prefab, new Vector3[] {
                        new Vector3(-1, 0.5f, -1),
                        new Vector3(1, 0.5f,1),
                        new Vector3(-1, 0.5f, 1),
                        new Vector3(1, 0.5f, -1),

                        new Vector3(-1, -0.5f, -1),
                        new Vector3(1, -0.5f, -1),
                    });
                    break;

                case "blackmarble_creep_stair":
                    SnapPointHelper.AddSnapPoints(prefab, new Vector3[] {
                        new Vector3(-1, 1, -1),
                        new Vector3(1, 1, -1),
                        new Vector3(-1, 0, -1),
                        new Vector3(1, 0, -1),
                        new Vector3(-1, 0, 1),
                        new Vector3(1, 0, 1),
                    });
                    break;

                case "blackmarble_floor_large":
                    for (int y = -1; y <= 1; y += 2)
                    {
                        for (int x = -4; x <= 4; x += 2)
                        {
                            for (int z = -4; z <= 4; z += 2)
                            {
                                pts.Add(new Vector3(x, y, z));
                            }
                        }
                    }
                    SnapPointHelper.AddSnapPoints(prefab, pts);
                    break;

                case "blackmarble_head_big01":
                    SnapPointHelper.AddSnapPoints(prefab, new Vector3[] {
                        new Vector3(-1, 1, -1),
                        new Vector3(1, 1,1),
                        new Vector3(-1, 1, 1),
                        new Vector3(1, 1, -1),

                        new Vector3(1, -1, -1),
                        new Vector3(-1, -1, -1),
                    });
                    break;

                case "blackmarble_head_big02":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(-1, 1, -1),
                            new Vector3(1, 1,1),
                            new Vector3(-1, 1, 1),
                            new Vector3(1, 1, -1),

                            new Vector3(1, -1, -1),
                            new Vector3(-1, -1, -1),
                        }
                    );
                    break;

                case "blackmarble_out_2":
                    // Fix piece colliders via layers
                    UnityEngine.Object.DestroyImmediate(prefab.GetComponent<MeshCollider>());
                    SnapPointHelper.FixPiece(prefab);
                    break;

                case "blackmarble_tile_floor_1x1":
                    prefab.transform.Find("_snappoint").gameObject.transform.localPosition = new Vector3(0.5f, 0.1f, 0.5f);
                    prefab.transform.Find("_snappoint (1)").gameObject.transform.localPosition = new Vector3(0.5f, 0.1f, -0.5f);
                    prefab.transform.Find("_snappoint (2)").gameObject.transform.localPosition = new Vector3(-0.5f, 0.1f, 0.5f);
                    prefab.transform.Find("_snappoint (3)").gameObject.transform.localPosition = new Vector3(-0.5f, 0.1f, -0.5f);
                    break;

                case "blackmarble_tile_floor_2x2":
                    prefab.transform.Find("_snappoint").gameObject.transform.localPosition = new Vector3(1, 0.1f, 1);
                    prefab.transform.Find("_snappoint (1)").gameObject.transform.localPosition = new Vector3(1, 0.1f, -1);
                    prefab.transform.Find("_snappoint (2)").gameObject.transform.localPosition = new Vector3(-1, 0.1f, 1);
                    prefab.transform.Find("_snappoint (3)").gameObject.transform.localPosition = new Vector3(-1, 0.1f, -1);
                    break;

                case "blackmarble_tile_wall_1x1":
                    prefab.transform.Find("_snappoint").gameObject.transform.localPosition = new Vector3(0.5f, 0, 0.1f);
                    prefab.transform.Find("_snappoint (1)").gameObject.transform.localPosition = new Vector3(0.5f, 1, 0.1f);
                    prefab.transform.Find("_snappoint (2)").gameObject.transform.localPosition = new Vector3(-0.5f, 0, 0.1f);
                    prefab.transform.Find("_snappoint (3)").gameObject.transform.localPosition = new Vector3(-0.5f, 1, 0.1f);
                    prefab.transform.Find("_snappoint (4)").gameObject.transform.localPosition = new Vector3(0.25f, 0, 0.1f);
                    prefab.transform.Find("_snappoint (5)").gameObject.transform.localPosition = new Vector3(0.25f, 0.5f, 0.1f);
                    prefab.transform.Find("_snappoint (6)").gameObject.transform.localPosition = new Vector3(-0.25f, 0, 0.1f);
                    prefab.transform.Find("_snappoint (7)").gameObject.transform.localPosition = new Vector3(-0.25f, 0.5f, 0.1f);
                    break;

                case "blackmarble_tile_wall_2x2":
                    prefab.transform.Find("_snappoint").gameObject.transform.localPosition = new Vector3(1, 0, 0.1f);
                    prefab.transform.Find("_snappoint (1)").gameObject.transform.localPosition = new Vector3(1, 2, 0.1f);
                    prefab.transform.Find("_snappoint (2)").gameObject.transform.localPosition = new Vector3(-1, 0, 0.1f);
                    prefab.transform.Find("_snappoint (3)").gameObject.transform.localPosition = new Vector3(-1, 2, 0.1f);
                    prefab.transform.Find("_snappoint (4)").gameObject.transform.localPosition = new Vector3(0.5f, 0, 0.1f);
                    prefab.transform.Find("_snappoint (5)").gameObject.transform.localPosition = new Vector3(0.5f, 1, 0.1f);
                    prefab.transform.Find("_snappoint (6)").gameObject.transform.localPosition = new Vector3(-0.5f, 0, 0.1f);
                    prefab.transform.Find("_snappoint (7)").gameObject.transform.localPosition = new Vector3(-0.5f, 1, 0.1f);
                    break;

                case "blackmarble_tile_wall_2x4":
                    prefab.transform.Find("_snappoint").gameObject.transform.localPosition = new Vector3(1, 0, 0.1f);
                    prefab.transform.Find("_snappoint (1)").gameObject.transform.localPosition = new Vector3(1, 4, 0.1f);
                    prefab.transform.Find("_snappoint (2)").gameObject.transform.localPosition = new Vector3(-1, 0, 0.1f);
                    prefab.transform.Find("_snappoint (3)").gameObject.transform.localPosition = new Vector3(-1, 4, 0.1f);
                    prefab.transform.Find("_snappoint (4)").gameObject.transform.localPosition = new Vector3(0.5f, 0, 0.1f);
                    prefab.transform.Find("_snappoint (5)").gameObject.transform.localPosition = new Vector3(0.5f, 2, 0.1f);
                    prefab.transform.Find("_snappoint (6)").gameObject.transform.localPosition = new Vector3(-0.5f, 0, 0.1f);
                    prefab.transform.Find("_snappoint (7)").gameObject.transform.localPosition = new Vector3(-0.5f, 2, 0.1f);
                    break;

                case "dungeon_queen_door":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(2.5f, 0, 0),
                            new Vector3(-2.5f, 0, 0),
                        }
                    );
                    break;

                case "dungeon_sunkencrypt_irongate":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(1, -0.4f, 0),
                            new Vector3(-1, -0.4f, 0),
                        }
                    );
                    break;

                case "sunken_crypt_gate":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(1, 0, 0),
                            new Vector3(-1, 0, 0),
                        }
                    );
                    break;

                case "dvergrprops_wood_beam":
                    /*
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(3, 0.45f, 0.45f),
                            new Vector3(3, 0.45f, -0.42f),
                            new Vector3(3, -0.45f, -0.42f),
                            new Vector3(3, -0.45f, -0.45f),

                            new Vector3(-3, 0.45f, 0.45f),
                            new Vector3(-3, 0.45f, -0.42f),
                            new Vector3(-3, -0.45f, -0.42f),
                            new Vector3(-3, -0.45f, -0.45f),
                        }
                    );
                    */
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(3, 0, 0),
                            new Vector3(-3, 0, 0),
                        }
                    );
                    break;

                case "dvergrprops_wood_pole":
                    /*
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(0.45f, 2, 0.45f),
                            new Vector3(-0.45f, 2, 0.45f),
                            new Vector3(0.45f, 2, -0.45f),
                            new Vector3(-0.45f, 2, -0.45f),
                            new Vector3(0.45f, -2, 0.45f),
                            new Vector3(-0.45f, -2, 0.45f),
                            new Vector3(0.45f, -2, -0.45f),
                            new Vector3(-0.45f, -2, -0.45f),
                        }
                    );
                    */
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(0, 2, 0),
                            new Vector3(0, -2, 0),
                        }
                    );
                    break;

                case "dvergrprops_wood_wall":
                    // Patch only the floor
                    /*generateSnapPoints(prefab, new Vector3[] {
                        new Vector3(2.2f, -2, -0.45f),
                        new Vector3(2.2f, -2, 0.45f),
                        new Vector3(-2.2f, -2, -0.45f),
                        new Vector3(-2.2f, -2, 0.45f),
                    });*/
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(2.2f, 2, 0),
                            new Vector3(-2.2f, 2, 0),
                            new Vector3(2.2f, -2, 0),
                            new Vector3(-2.2f, -2, 0),
                        }
                    );
                    break;

                case "dvergrtown_arch":
                    /*
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(1, 0, -0.5f),
                            new Vector3(1, 0, 0.5f),
                            new Vector3(1, 1, -0.5f),
                            new Vector3(1, 1, 0.5f),

                            new Vector3(-1, 1, -0.5f),
                            new Vector3(-1, 1, 0.5f),

                            new Vector3(-1, -1, 0.5f),
                            new Vector3(-1, -1, -0.5f),
                            new Vector3(0, -1, 0.5f),
                            new Vector3(0, -1, -0.5f),
                        }
                    );
                    */
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(1, 0.5f, 0),
                        }
                    );
                    break;

                case "dvergrtown_secretdoor":
                    /*
                     SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(2, 0, -0.35f),
                            new Vector3(2, 0, 0.4f),
                            new Vector3(-2, 0, -0.35f),
                            new Vector3(-2, 0, 0.4f),

                            new Vector3(2, 4, -0.35f),
                            new Vector3(2, 4, 0.4f),
                            new Vector3(-2, 4, -0.35f),
                            new Vector3(-2, 4, 0.4f),
                        }
                    );
                    */
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(2, 0, 0),
                            new Vector3(-2, 0, 0),
                            new Vector3(2, 4, 0),
                            new Vector3(-2, 4, 0),
                        }
                    );
                    break;

                case "dvergrtown_slidingdoor":
                    /*
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(2, 0, 0),
                            new Vector3(2, 0, 0.2f),
                            new Vector3(-2, 0, -0.2f),
                            new Vector3(-2, 0, 0.2f),

                            new Vector3(2, 4, -0.2f),
                            new Vector3(2, 4, 0.2f),
                            new Vector3(-2, 4, -0.2f),
                            new Vector3(-2, 4, 0.2f),
                        }
                    );
                    */
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(2, 0, 0),
                            new Vector3(-2, 0, 0),
                            new Vector3(2, 4, 0),
                            new Vector3(-2, 4, 0),
                        }
                    );
                    break;

                case "dvergrtown_stair_corner_wood_left":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(0.25f, 0, -0.25f),
                            new Vector3(0.25f, 0, 0.25f),
                            new Vector3(0.25f, 1.1f, -0.25f),
                            new Vector3(0.25f, 1.1f, 0.25f),

                            new Vector3(0.25f, 0, 2),
                            new Vector3(-0.25f, 1.1f, -0.25f),

                            new Vector3(-2, 1.1f, -0.25f)
                        }
                    );
                    break;

                case "dvergrprops_shelf":
                case "dvergrprops_table":
                    var wearNTear = prefab.GetComponent<WearNTear>();
                    if (wearNTear != null)
                    {
                        // allow these pieces to support other pieces
                        wearNTear.m_supports = true;
                    }
                    break;

                case "dvergrtown_wood_beam":
                    /*
                     SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(3, 0.45f, 0.45f),
                            new Vector3(3, 0.45f, -0.42f),
                            new Vector3(3, -0.45f, -0.42f),
                            new Vector3(3, -0.45f, -0.45f),

                            new Vector3(-3, 0.45f, 0.45f),
                            new Vector3(-3, 0.45f, -0.42f),
                            new Vector3(-3, -0.45f, -0.42f),
                            new Vector3(-3, -0.45f, -0.45f),
                        }
                    );
                    */
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(3, 0, 0),
                            new Vector3(-3, 0, 0),
                        }
                    );
                    break;

                case "dvergrtown_wood_pole":
                    /*
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(0.45f, 2, 0.45f),
                            new Vector3(-0.45f, 2, 0.45f),
                            new Vector3(0.45f, 2, -0.45f),
                            new Vector3(-0.45f, 2, -0.45f),
                            new Vector3(0.45f, -2, 0.45f),
                            new Vector3(-0.45f, -2, 0.45f),
                            new Vector3(0.45f, -2, -0.45f),
                            new Vector3(-0.45f, -2, -0.45f),
                        }
                    );
                    */
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(0, -2, 0),
                            new Vector3(0, 2, 0),
                        }
                    );
                    break;

                case "dvergrtown_wood_stake":
                    // Patch only the floor
                    /*
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(0.15f, 0, 0.15f),
                            new Vector3(-0.15f, 0, 0.15f),
                            new Vector3(0.15f, 0, -0.15f),
                            new Vector3(-0.15f, 0, -0.15f),
                        }
                    );
                    */
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(0, 0, 0),
                        }
                    );
                    break;

                case "dvergrtown_wood_crane":
                    /*
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(0.4f, -3, 0.4f),
                            new Vector3(-0.4f, -3, 0.4f),
                            new Vector3(0.4f, -3, -0.4f),
                            new Vector3(-0.4f, -3, -0.4f),
                        }
                    );
                    */
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(0, -3, 0),
                        }
                    );
                    break;

                case "dvergrtown_wood_support":
                    /*
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(-2.4f, 0, -0.4f),
                            new Vector3(-2.4f, 0, 0.4f),
                            new Vector3(-1.6f, 0, -0.4f),
                            new Vector3(-1.6f, 0, 0.4f),

                            new Vector3(2.4f, 0, -0.4f),
                            new Vector3(2.4f, 0, 0.4f),
                            new Vector3(1.6f, 0, -0.4f),
                            new Vector3(1.6f, 0, 0.4f),
                        }
                    );
                    */
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(-2, 0, 0),
                            new Vector3(2, 0, 0),
                        }
                    );
                    break;

                case "dvergrtown_wood_wall01":
                    // Fix collider y
                    prefab.transform.Find("wallcollider").transform.localPosition = new Vector3(0, 0, 0);
                    /*
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                        new Vector3(3, -2.7f, -0.45f),
                        new Vector3(3, -2.7f, 0.45f),
                        new Vector3(-3, -2.7f, -0.45f),
                        new Vector3(-3, -2.7f, 0.45f),

                        new Vector3(3, 2.7f, -0.45f),
                        new Vector3(3, 2.7f, 0.45f),
                        new Vector3(-3, 2.7f, -0.45f),
                        new Vector3(-3, 2.7f, 0.45f),
                        }
                    );
                    */
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(-3, -2.7f, 0),
                            new Vector3(3, -2.7f, 0),
                            new Vector3(-3, 2.7f, 0),
                            new Vector3(3, 2.7f, 0),
                        }
                    );
                    break;

                case "dvergrtown_wood_wall02":
                    /*

                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(3, -0.6f, -0.25f),
                            new Vector3(3, -0.6f, 0.25f),
                            new Vector3(-3, -0.6f, -0.25f),
                            new Vector3(-3, -0.6f, 0.25f),

                            new Vector3(3, 4.6f, -0.25f),
                            new Vector3(3, 4.6f, 0.25f),
                            new Vector3(-3, 4.6f, -0.25f),
                            new Vector3(-3, 4.6f, 0.25f),
                        }
                    );
                    */

                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(-3, -0.5f, 0),
                            new Vector3(3, -0.5f, 0),
                            new Vector3(-3, 4.5f, 0),
                            new Vector3(3, 4.5f, 0),
                        }
                    );
                    break;

                case "dvergrtown_wood_wall03":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(1.1f, 0, 0),
                            new Vector3(-1.1f, 0, 0),

                            new Vector3(2, 2, 0),
                            new Vector3(-2, 2, 0),

                            new Vector3(1.1f, 4, 0),
                            new Vector3(-1.1f, 4, 0),
                        }
                    );
                    break;

                case "goblin_roof_45d":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(1, 0, 1),
                            new Vector3(-1, 0, 1),
                            new Vector3(1, 2, -1),
                            new Vector3(-1, 2, -1),
                        }
                    );
                    break;

                case "goblin_roof_45d_corner":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(-1, 0, -1),
                            new Vector3(1, 0, 1),
                        }
                    );
                    break;

                case "goblin_woodwall_1m":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(-0.5f, 0, 0),
                            new Vector3(0.5f, 0, 0),
                            new Vector3(-0.5f, 2, 0),
                            new Vector3(0.5f, 2, 0),
                        }
                    );
                    break;

                case "goblin_woodwall_2m":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(-1, 0, 0),
                            new Vector3(1, 0, 0),
                            new Vector3(-1, 2, 0),
                            new Vector3(1, 2, 0),
                        }
                    );
                    break;

                case "Ice_floor":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(2, 1, 2),
                            new Vector3(-2, 1, -2),
                            new Vector3(2, 1, -2),
                            new Vector3(-2, 1, 2),

                            new Vector3(2, -1, 2),
                            new Vector3(-2, -1, -2),
                            new Vector3(2, -1, -2),
                            new Vector3(-2, -1, 2),
                        }
                    );
                    break;

                case "turf_roof": // 26 degree
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[]
                        {
                            new Vector3(0.0f, 0.5f, 0.0f), // center
                            new Vector3(0.0f, 0.0f, 1.0f), // Bottom mid edge
                            new Vector3(0.0f, 1.0f, -1.0f), // top mid edge
                            new Vector3(1.0f, 0.5f, 0.0f), // left mid edge
                            new Vector3(-1.0f, 0.5f, 0.0f), // right mid edge
                        }
                    );
                    break;

                case "turf_roof_top":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[]
                        {
                            new Vector3(0.0f, 0.0f, 1.0f),  // side mid edge
                            new Vector3(0.0f, 0.0f, -1.0f),  // side mid edge
                            new Vector3(0.0f, 0.5f, 0.0f),  // top mid edge
                        }
                    );
                    break;

                case "metalbar_1x2":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[]
                        {
                            new Vector3(0.0f, 0.0f, 0.0f),
                            new Vector3(1.0f, -1.0f, 0.0f),
                            new Vector3(1.0f, 0.0f, 0.0f),
                            new Vector3(-1.0f, -1.0f, 0.0f),
                            new Vector3(-1.0f, 0.0f, 0.0f),
                        }
                    );
                    break;

                case "stone_floor":
                    for (float y = -0.5f; y <= 0.5f; y += 1)
                    {
                        for (int x = -2; x <= 2; x += 1)
                        {
                            for (int z = -2; z <= 2; z += 1)
                            {
                                if (!(Math.Abs(x) == 2 && Mathf.Abs(z) == 2))  // skip corners that already have snap points
                                {
                                    pts.Add(new Vector3(x, y, z));
                                }
                            }
                        }
                    }
                    SnapPointHelper.AddSnapPoints(prefab, pts);
                    break;

                case "stoneblock_fracture":
                    // x and y scale is strange for this one
                    for (float y = -0.5f; y <= 0.5f; y += 1)
                    {
                        for (int x = -1; x <= 1; x += 1)
                        {
                            for (int z = -1; z <= 1; z += 1)
                            {
                                {
                                    pts.Add(new Vector3(x, y, z));
                                }
                            }
                        }
                    }
                    SnapPointHelper.AddSnapPoints(prefab, pts, true);
                    UnityEngine.Object.DestroyImmediate(prefab.GetComponent<MineRock>());
                    CollisionHelper.AddBoxCollider(prefab, Vector3.zero, new Vector3(2, 1, 2));
                    break;

                case "blackmarble_post01":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[]
                        {
                            new Vector3(0.0f, 0.0f, 0.0f),
                            // these snap points are outside the rigid body
                            //new Vector3(1.0f, -1.0f, 1.0f),
                            //new Vector3(1.0f, -1.0f, -1.0f),
                            //new Vector3(-1.0f, -1.0f, -1.0f),
                            //new Vector3(-1.0f, -1.0f, 1.0f),
                            new Vector3(1.0f, 0.0f, 1.0f),
                            new Vector3(1.0f, 0.0f, -1.0f),
                            new Vector3(-1.0f, 0.0f, -1.0f),
                            new Vector3(-1.0f, 0.0f, 1.0f),
                            new Vector3(1.0f, 1.0f, 1.0f),
                            new Vector3(1.0f, 1.0f, -1.0f),
                            new Vector3(-1.0f, 1.0f, -1.0f),
                            new Vector3(-1.0f, 1.0f, 1.0f),
                            new Vector3(0.0f, 3.5f, 0.0f), // for torches
                        },
                        true
                    );
                    foreach (var collider in prefab.GetComponentsInChildren<BoxCollider>())
                    {
                        if (collider.name == "collider")
                        {
                            collider.center += new Vector3(0.0f, 0.5f, 0.0f);
                            collider.size += new Vector3(0.0f, -1.0f, 0.0f);
                        }
                    }
                    break;

                case "wood_ledge":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[]
                        {
                            new Vector3(0.5f, 0.0f, 0.25f),
                            new Vector3(0.0f, 0.0f, 0.25f),
                            new Vector3(-0.5f, 0.0f, 0.25f),
                            new Vector3(-0.5f, 0.0f, -0.25f),
                            new Vector3(0.5f, 0.0f, -0.25f),
                        }
                    );
                    break;

                case "dverger_demister":
                    CollisionHelper.RemoveColliders(prefab); //remove large box collider

                    // add thin collider along post
                    CollisionHelper.AddBoxCollider(prefab, new Vector3(0.0f, -0.2f, 0.0f), new Vector3(0.2f, 1.1f, 0.2f));

                    // add collider around the ring
                    CollisionHelper.AddBoxCollider(prefab, Vector3.zero, new Vector3(1.0f, 0.1f, 1.0f));

                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[]
                        {
                            new Vector3(0.0f, 0.0f, 0.0f),
                            new Vector3(0.0f, -0.65f, 0.0f),
                        }
                    );
                    break;

                case "dverger_demister_large":
                    CollisionHelper.RemoveColliders(prefab); //remove large box collider

                    // add thin collider along post
                    CollisionHelper.AddBoxCollider(prefab, new Vector3(0.0f, -0.25f, 0.0f), new Vector3(0.2f, 1.3f, 0.2f));

                    // add collider around the ring
                    CollisionHelper.AddBoxCollider(prefab, new Vector3(0.0f, -0.25f, 0.0f), new Vector3(1.0f, 0.1f, 1.0f));

                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[]
                        {
                            new Vector3(0.0f, 0.0f, 0.0f),
                            new Vector3(0.0f, -0.9f, 0.0f),
                        }
                    );
                    break;

                case "dvergrprops_hooknchain":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[]
                        {
                            new Vector3(0.0f, 2.5f, 0.0f)
                        }
                    );
                    break;

                case "barrell":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[]
                        {
                            new Vector3(0.0f, -1.0f, 0.0f)
                        }
                    );
                    break;

                default:
                    // prefab may not have a piece component
                    //Transform transform = prefab.GetComponent<Piece>().transform;

                    // Add SnapPoint to Local Center if there is not already one present there
                    Transform transform = prefab.transform;
                    for (var index = 0; index < transform.childCount; ++index)
                    {
                        var child = transform.GetChild(index);
                        if (child.CompareTag("snappoint"))
                        {
                            if (child.localPosition.Equals(Vector3.zero))
                            {
                                return;
                            }
                        }
                    }
                    SnapPointHelper.AddSnapPointToCenter(prefab);
                    break;
            }
        }
    }
}