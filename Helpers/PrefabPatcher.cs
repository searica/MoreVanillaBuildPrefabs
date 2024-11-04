// Ignore Spelling: MVBP

using Jotunn.Managers;
using MVBP.Extensions;
using MVBP.Models;
using System;
using System.Collections.Generic;
using UnityEngine;
using static MVBP.Helpers.SnapPointNames;

namespace MVBP.Helpers
{
    internal static class PrefabPatcher
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
                    SnapPointHelper.AddSnapPointToOrigin(prefab);
                    try
                    {
                        GameObject body = prefab?.transform?.Find("Player Pose")?.Find("Visual")?.Find("body")?.gameObject;
                        SkinnedMeshRenderer meshRender = body.GetComponent<SkinnedMeshRenderer>();
                        meshRender.sharedMaterial = TextureHelper.GetCustomArmorStandMaterial();
                    }
                    catch
                    {
                        Log.LogWarning($"Failed to patch material for {prefab.name}");
                    }
                    break;

                case "Trailership":
                    // Fix hull
                    MeshFilter meshFilter = prefab.FindDeepChild("hull").GetComponent<MeshFilter>();
                    GameObject longShip = ZNetScene.instance?.GetPrefab("VikingShip");
                    MeshFilter lsMeshFilter = longShip.FindDeepChild("hull").GetComponent<MeshFilter>();
                    if (meshFilter && lsMeshFilter)
                    {
                        meshFilter.mesh = lsMeshFilter.mesh;
                    }

                    // Fix shields
                    GameObject shieldBanded = ZNetScene.instance?.GetPrefab("ShieldBanded");
                    Material mat = shieldBanded.GetComponentInChildren<MeshRenderer>().material;
                    Transform storage = prefab.transform.Find("ship").Find("visual").Find("Customize").Find("storage");
                    if (mat && storage)
                    {
                        int children = storage.childCount;
                        for (int i = 0; i < children; ++i)
                        {
                            Transform child = storage.GetChild(i);
                            if (child && child.name.StartsWith("Shield"))
                            {
                                child.GetComponent<MeshRenderer>().material = mat;
                            }
                        }
                    }

                    // Fix Sail cloth
                    Cloth cloth = prefab.FindDeepChild("sail_full").GetComponent<Cloth>();
                    Cloth lsCloth = longShip.FindDeepChild("sail_full").GetComponent<Cloth>();
                    if (cloth && lsCloth)
                    {
                        cloth.coefficients = lsCloth.coefficients;
                    }

                    // Fix missing control GUI position
                    Ship ship = prefab.GetComponent<Ship>();
                    if (ship)
                    {
                        GameObject controlGui = new("ControlGui");
                        controlGui.transform.parent = prefab.transform;
                        controlGui.transform.localPosition = new Vector3(1.0f, 1.696f, -6.54f);
                        ship.m_controlGuiPos = controlGui.transform;

                        // Fix sail not catching wind
                        ship.m_sailForceOffset = 2; // same as longship
                        ship.m_sailForceFactor = 0.05f; // same as longship
                    }

                    // Fix missing rudder button attachpoint
                    ShipControlls shipControls = prefab.FindDeepChild("rudder_button").GetComponent<ShipControlls>();
                    Transform rudderAttach = prefab.transform.Find("sit locations").Find("sit_box (4)").Find("attachpoint");
                    if (shipControls && rudderAttach)
                    {
                        shipControls.m_attachPoint = rudderAttach.transform;
                    }

                    break;

                // Causes the chest to break if loaded without mod
                //case "TreasureChest_dvergr_loose_stone":
                //    var boxCollider = gameObject.AddComponent<BoxCollider>();
                //    boxCollider.size = new Vector3(2, 1, 2);
                //    SnapPointHelper.AddSnapPointsToBoxColliderCorners(gameObject, boxCollider);
                //    break;

                case "TreasureChest_mountaincave":
                case "TreasureChest_trollcave":
                    SnapPointHelper.FixPieceLayers(prefab);
                    SnapPointHelper.AddSnapPointsToMeshCorners(prefab, "stonechest");
                    break;
                //case "TreasureChest_dvergrtown":
                //    break;
                //case "TreasureChest_dvergrtower":
                //    break;
                case "TreasureChest_plains_stone":
                case "TreasureChest_fCrypt":
                case "TreasureChest_sunkencrypt":
                    SnapPointHelper.FixPieceLayers(prefab);
                    SnapPointHelper.AddSnapPoints(
                       prefab,
                       new Vector3[] {
                            new(0.0f, -0.01f, 0.0f),
                            new(1.0f, -0.01f, 0.37f),
                            new(1.0f, -0.01f, -0.37f),
                            new(-1.0f, -0.01f, 0.37f),
                            new(-1.0f, -0.01f, -0.37f),
                            new(0.65f, 0.8f, 0.35f),
                            new(0.65f, 0.8f, -0.35f),
                            new(-0.65f, 0.8f, 0.35f),
                            new(-0.65f, 0.8f, -0.35f)
                       }
                    );
                    break;

                case "blackmarble_column_3":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new(-1, 4, -1),
                            new(1, 4, 1),
                            new(-1, 4, 1),
                            new(1, 4, -1),

                            new(-1, 2, -1),
                            new(1, 2, 1),
                            new(-1, 2, 1),
                            new(1, 2, -1),

                            new(-1, 0, -1),
                            new(1, 0, 1),
                            new(-1, 0, 1),
                            new(1, 0, -1),

                            new(-1, -2, -1),
                            new(1, -2, 1),
                            new(-1, -2, 1),
                            new(1, -2, -1),

                            new(-1, -4, -1),
                            new(1, -4, 1),
                            new(-1, -4, 1),
                            new(1, -4, -1),
                        }
                    );
                    break;

                case "blackmarble_creep_4x1x1":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new(-0.5f, 2, -0.5f),
                            new(0.5f, 2, 0.5f),
                            new(-0.5f, 2, 0.5f),
                            new(0.5f, 2, -0.5f),

                            new(-0.5f, -2, -0.5f),
                            new(0.5f, -2, 0.5f),
                            new(-0.5f, -2, 0.5f),
                            new(0.5f, -2, -0.5f),

                            new(0, -2, 0),
                            new(0, 2, 0),
                        }
                    );

                    // ? Place the piece randomly in horizontal or vertical position ?
                    UnityEngine.Object.DestroyImmediate(prefab.transform.Find("new").gameObject.GetComponent<RandomPieceRotation>());

                    break;

                case "blackmarble_creep_4x2x1":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new(1, 2, -0.5f),
                            new(1, 2, 0.5f),
                            new(-1, 2, -0.5f),
                            new(-1, 2, 0.5f),

                            new(1, -2, -0.5f),
                            new(1, -2, 0.5f),
                            new(-1, -2, -0.5f),
                            new(-1, -2, 0.5f),

                            new(0, -2, 0),
                            new(0, 2, 0),
                        }
                    );

                    // ? Place the piece randomly in horizontal or vertical position ?
                    UnityEngine.Object.DestroyImmediate(prefab.transform.Find("new").gameObject.GetComponent<RandomPieceRotation>());
                    break;

                case "blackmarble_creep_slope_inverted_1x1x2":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new(-0.5f, 1, -0.5f),
                            new(0.5f, 1, 0.5f),
                            new(-0.5f, 1, 0.5f),
                            new(0.5f, 1, -0.5f),

                            new(0.5f, -1, -0.5f),
                            new(-0.5f, -1, -0.5f),
                        }
                    );
                    break;

                case "blackmarble_creep_slope_inverted_2x2x1":
                    SnapPointHelper.AddSnapPoints(prefab, new Vector3[] {
                        new(-1, 0.5f, -1),
                        new(1, 0.5f,1),
                        new(-1, 0.5f, 1),
                        new(1, 0.5f, -1),

                        new(-1, -0.5f, -1),
                        new(1, -0.5f, -1),
                    });
                    break;

                case "blackmarble_creep_stair":
                    SnapPointHelper.AddSnapPoints(prefab, new Vector3[] {
                        new(-1, 1, -1),
                        new(1, 1, -1),
                        new(-1, 0, -1),
                        new(1, 0, -1),
                        new(-1, 0, 1),
                        new(1, 0, 1),
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
                        new(-1, 1, -1),
                        new(1, 1,1),
                        new(-1, 1, 1),
                        new(1, 1, -1),

                        new(1, -1, -1),
                        new(-1, -1, -1),
                    });
                    break;

                case "blackmarble_head_big02":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new(-1, 1, -1),
                            new(1, 1,1),
                            new(-1, 1, 1),
                            new(1, 1, -1),

                            new(1, -1, -1),
                            new(-1, -1, -1),
                        }
                    );
                    break;

                case "blackmarble_out_2":
                    // Fix piece colliders via layers
                    UnityEngine.Object.DestroyImmediate(prefab.GetComponent<MeshCollider>());
                    SnapPointHelper.FixPieceLayers(prefab);
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
                    prefab.transform.Find("_snappoint").gameObject.transform.localPosition = new Vector3(0.5f, -0.05f, 0.1f);
                    prefab.transform.Find("_snappoint (1)").gameObject.transform.localPosition = new Vector3(0.5f, 0.95f, 0.1f);
                    prefab.transform.Find("_snappoint (2)").gameObject.transform.localPosition = new Vector3(-0.5f, -0.05f, 0.1f);
                    prefab.transform.Find("_snappoint (3)").gameObject.transform.localPosition = new Vector3(-0.5f, 0.95f, 0.1f);
                    prefab.transform.Find("_snappoint (4)").gameObject.transform.localPosition = new Vector3(0, -0.05f, 0.1f);
                    prefab.transform.Find("_snappoint (5)").gameObject.transform.localPosition = new Vector3(0.5f, 0.45f, 0.1f);
                    prefab.transform.Find("_snappoint (6)").gameObject.transform.localPosition = new Vector3(0, 0.95f, 0.1f);
                    prefab.transform.Find("_snappoint (7)").gameObject.transform.localPosition = new Vector3(-0.5f, 0.45f, 0.1f);
                    break;

                case "blackmarble_tile_wall_2x2":
                    prefab.transform.Find("_snappoint").gameObject.transform.localPosition = new Vector3(1, -0.05f, 0.1f);
                    prefab.transform.Find("_snappoint (1)").gameObject.transform.localPosition = new Vector3(1, 1.95f, 0.1f);
                    prefab.transform.Find("_snappoint (2)").gameObject.transform.localPosition = new Vector3(-1, -0.05f, 0.1f);
                    prefab.transform.Find("_snappoint (3)").gameObject.transform.localPosition = new Vector3(-1, -0.05f, 0.1f);
                    prefab.transform.Find("_snappoint (4)").gameObject.transform.localPosition = new Vector3(0.5f, -0.05f, 0.1f);
                    prefab.transform.Find("_snappoint (5)").gameObject.transform.localPosition = new Vector3(0.5f, 0.95f, 0.1f);
                    prefab.transform.Find("_snappoint (6)").gameObject.transform.localPosition = new Vector3(-0.5f, -0.05f, 0.1f);
                    prefab.transform.Find("_snappoint (7)").gameObject.transform.localPosition = new Vector3(-0.5f, 0.95f, 0.1f);
                    break;

                case "blackmarble_tile_wall_2x4":
                    prefab.transform.Find("_snappoint").gameObject.transform.localPosition = new Vector3(1, -0.05f, 0.1f);
                    prefab.transform.Find("_snappoint (1)").gameObject.transform.localPosition = new Vector3(1, 3.95f, 0.1f);
                    prefab.transform.Find("_snappoint (2)").gameObject.transform.localPosition = new Vector3(-1, -0.05f, 0.1f);
                    prefab.transform.Find("_snappoint (3)").gameObject.transform.localPosition = new Vector3(-1, 3.95f, 0.1f);
                    prefab.transform.Find("_snappoint (4)").gameObject.transform.localPosition = new Vector3(0.5f, -0.05f, 0.1f);
                    prefab.transform.Find("_snappoint (5)").gameObject.transform.localPosition = new Vector3(0.5f, 1.95f, 0.1f);
                    prefab.transform.Find("_snappoint (6)").gameObject.transform.localPosition = new Vector3(-0.5f, -0.05f, 0.1f);
                    prefab.transform.Find("_snappoint (7)").gameObject.transform.localPosition = new Vector3(-0.5f, 1.95f, 0.1f);
                    break;

                case "dungeon_queen_door":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new(2.5f, 0, 0),
                            new(-2.5f, 0, 0),
                        }
                    );
                    break;

                case "dungeon_sunkencrypt_irongate":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new(1, -0.4f, 0),
                            new(-1, -0.4f, 0),
                        }
                    );
                    break;

                case "sunken_crypt_gate":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new(1, 0, 0),
                            new(-1, 0, 0),
                        }
                    );
                    break;

                case "dvergrprops_wood_beam":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new(3, 0, 0),
                            new(-3, 0, 0),
                        }
                    );
                    break;

                case "dvergrprops_wood_pole":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new(0, 2, 0),
                            new(0, -2, 0),
                        }
                    );
                    break;

                case "dvergrprops_wood_wall":
                    // Patch only the floor
                    /*generateSnapPoints(gameObject, new Vector3[] {
                        new(2.2f, -2, -0.45f),
                        new(2.2f, -2, 0.45f),
                        new(-2.2f, -2, -0.45f),
                        new(-2.2f, -2, 0.45f),
                    });*/
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new(2.2f, 2, 0),
                            new(-2.2f, 2, 0),
                            new(2.2f, -2, 0),
                            new(-2.2f, -2, 0),
                        }
                    );
                    break;

                case "dvergrtown_arch":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new(1, 0.5f, 0),
                        }
                    );
                    break;

                case "dvergrtown_secretdoor":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new(2, 0, 0),
                            new(-2, 0, 0),
                            new(2, 4, 0),
                            new(-2, 4, 0),
                        }
                    );
                    break;

                case "dvergrtown_slidingdoor":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new(2, 0, 0),
                            new(-2, 0, 0),
                            new(2, 4, 0),
                            new(-2, 4, 0),
                        }
                    );
                    break;

                case "dvergrtown_stair_corner_wood_left":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new(0.25f, 0, -0.25f),
                            new(0.25f, 0, 0.25f),
                            new(0.25f, 1.1f, -0.25f),
                            new(0.25f, 1.1f, 0.25f),

                            new(0.25f, 0, 2),
                            new(-0.25f, 1.1f, -0.25f),

                            new(-2, 1.1f, -0.25f)
                        }
                    );
                    break;

                case "dvergrprops_shelf":
                case "dvergrprops_table":
                    WearNTear wearNTear = prefab.GetComponent<WearNTear>();
                    if (wearNTear)
                    {
                        wearNTear.m_supports = true; // allow these pieces to support other pieces
                    }
                    break;

                case "dvergrtown_wood_beam":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new(3, 0, 0),
                            new(-3, 0, 0),
                        }
                    );
                    break;

                case "dvergrtown_wood_pole":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new(0, -2, 0),
                            new(0, 2, 0),
                        }
                    );
                    break;

                case "dvergrtown_wood_stake":
                    // Patch only the floor
                    /*
                    SnapPointHelper.AddSnapPoints(
                        gameObject,
                        new Vector3[] {
                            new(0.15f, 0, 0.15f),
                            new(-0.15f, 0, 0.15f),
                            new(0.15f, 0, -0.15f),
                            new(-0.15f, 0, -0.15f),
                        }
                    );
                    */
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            Vector3.zero,
                        }
                    );
                    break;

                case "dvergrtown_wood_crane":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new(0, -3, 0),
                        }
                    );
                    break;

                case "dvergrtown_wood_support":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new(-2, 0, 0),
                            new(2, 0, 0),
                        }
                    );
                    break;

                case "dvergrtown_wood_wall01":
                    // Fix collider y
                    prefab.transform.Find("wallcollider").transform.localPosition = Vector3.zero;
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new(-3, -2.7f, 0),
                            new(3, -2.7f, 0),
                            new(-3, 2.7f, 0),
                            new(3, 2.7f, 0),
                        }
                    );
                    break;

                case "dvergrtown_wood_wall02":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new(-3, -0.5f, 0),
                            new(3, -0.5f, 0),
                            new(-3, 4.5f, 0),
                            new(3, 4.5f, 0),
                        }
                    );
                    break;

                case "dvergrtown_wood_wall03":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new(1.1f, 0, 0),
                            new(-1.1f, 0, 0),

                            new(2, 2, 0),
                            new(-2, 2, 0),

                            new(1.1f, 4, 0),
                            new(-1.1f, 4, 0),
                        }
                    );
                    break;

                case "goblin_roof_45d":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new(1, 0, 1),
                            new(-1, 0, 1),
                            new(1, 2, -1),
                            new(-1, 2, -1),
                        }
                    );
                    break;

                case "goblin_roof_45d_corner":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new(-1, 0, -1),
                            new(1, 0, 1),
                        }
                    );
                    break;

                case "goblin_woodwall_1m":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new(-0.5f, 0, 0),
                            new(0.5f, 0, 0),
                            new(-0.5f, 2, 0),
                            new(0.5f, 2, 0),
                        }
                    );
                    break;

                case "goblin_woodwall_2m":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new(-1, 0, 0),
                            new(1, 0, 0),
                            new(-1, 2, 0),
                            new(1, 2, 0),
                        }
                    );
                    break;

                case "Ice_floor":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new(2, 1, 2),
                            new(-2, 1, -2),
                            new(2, 1, -2),
                            new(-2, 1, 2),

                            new(2, -1, 2),
                            new(-2, -1, -2),
                            new(2, -1, -2),
                            new(-2, -1, 2),
                        }
                    );
                    break;

                case "turf_roof_top":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[]
                        {
                            // mid edge points get added by ExtraSnapPointsMadeEasy
                            new(1.0f, 0.5f, 0.0f),  // top front
                            new(-1.0f, 0.5f, 0.0f),  // top back
                        }
                    );
                    break;

                case "metalbar_1x2":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[]
                        {
                            new(0.0f, 0.0f, 0.0f),
                            new(1.0f, -1.0f, 0.0f),
                            new(1.0f, 0.0f, 0.0f),
                            new(-1.0f, -1.0f, 0.0f),
                            new(-1.0f, 0.0f, 0.0f),
                        }
                    );
                    break;

                case "stone_floor":
                    for (float y = -0.5f; y <= 0.5f; y += 1)
                    {
                        for (int x = -2; x <= 2; x += 1)
                        {
                            for (int z = -2; z <= 2; z += 1)
                            {   // skip corners that already have snap points
                                if (!(Math.Abs(x) == 2 && Mathf.Abs(z) == 2))
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
                    SnapPointHelper.FixPieceLayers(prefab);
                    SnapPointHelper.AddSnapPoints(prefab, pts);

                    // Can keep the MineRock component now that I handle the item drops properly
                    // UnityEngine.Object.DestroyImmediate(prefab.GetComponent<MineRock>());

                    CollisionHelper.AddBoxCollider(prefab, Vector3.zero, new Vector3(2, 1, 2));
                    break;

                case "blackmarble_post01":
                    SnapPointHelper.FixPieceLayers(prefab);
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[]
                        {
                            new(0.0f, 0.0f, 0.0f),
                            new(1.0f, 0.0f, 1.0f),
                            new(1.0f, 0.0f, -1.0f),
                            new(-1.0f, 0.0f, -1.0f),
                            new(-1.0f, 0.0f, 1.0f),
                            new(1.0f, 1.0f, 1.0f),
                            new(1.0f, 1.0f, -1.0f),
                            new(-1.0f, 1.0f, -1.0f),
                            new(-1.0f, 1.0f, 1.0f),
                            new(0.0f, 3.5f, 0.0f), // for demisters/torches
                        }
                    );
                    foreach (BoxCollider collider in prefab.GetComponentsInChildren<BoxCollider>())
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
                            new(0.5f, 0.0f, 0.25f),
                            new(0.0f, 0.0f, 0.25f),
                            new(-0.5f, 0.0f, 0.25f),
                            new(-0.5f, 0.0f, -0.25f),
                            new(0.5f, 0.0f, -0.25f),
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
                            new(0.0f, 0.0f, 0.0f),
                            new(0.0f, -0.65f, 0.0f),
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
                            new(0.0f, 0.0f, 0.0f),
                            new(0.0f, -0.9f, 0.0f),
                        }
                    );
                    break;

                case "dvergrprops_hooknchain":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new NamedSnapPoint[]
                        {
                            new(0.0f, 2.5f, 0.0f, TOP),
                            new(0.0f,-2.0f, 0.0f, "Hook") // TODO: See how this behaves and if I want to keep it.
                        }
                    );
                    break;

                case "barrell":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[]
                        {
                            new(0.0f, -1.0f, 0.0f)
                        }
                    );
                    break;

                case "goblin_strawpile":
                    // needs collider to allow removal
                    CollisionHelper.AddBoxCollider(
                        prefab,
                        Vector3.zero,
                        new Vector3(1.5f, 0.02f, 1.5f)
                    );
                    break;

                case "mountainkit_chair":
                    {
                        Chair chair = prefab.AddComponent<Chair>();
                        GameObject attachPoint = new("attachPoint");
                        attachPoint.transform.parent = prefab.transform;
                        attachPoint.transform.localPosition = Vector3.zero;
                        chair.m_attachPoint = attachPoint.transform;
                    }
                    break;

                case "dvergrprops_chair":
                    {
                        Chair chair = prefab.AddComponent<Chair>();
                        GameObject attachPoint = new("attachPoint");
                        attachPoint.transform.parent = prefab.transform;
                        attachPoint.transform.localPosition = new Vector3(0.0f, -0.15f, 0.0f);
                        chair.m_attachPoint = attachPoint.transform;
                    }

                    break;

                case "dvergrprops_stool":
                    {
                        Chair chair = prefab.AddComponent<Chair>();
                        GameObject attachPoint = new("attachPoint");
                        attachPoint.transform.parent = prefab.transform;
                        attachPoint.transform.localPosition = new Vector3(0.0f, -0.1f, 0.0f);
                        chair.m_attachPoint = attachPoint.transform;
                    }
                    break;
                case "Ashland_Stair":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new NamedSnapPoint[]
                        {
                            new( 2.0f, 0.0f, -2.0f, $"{BOTTOM} 1"),
                            new(-2.0f, 0.0f, -2.0f, $"{BOTTOM} 2"),
                            new(-2.0f, 0.0f,  2.0f, $"{BOTTOM} 3"),
                            new( 2.0f, 0.0f,  2.0f, $"{BOTTOM} 4"),
                            new( 2.0f, 2.0f, -2.0f, $"{TOP} 1"),
                            new(-2.0f, 2.0f, -2.0f, $"{TOP} 2"),
                        });
                    break;
                case "Ashlands_Fortress_Floor":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new NamedSnapPoint[]
                        {
                            new( 1.0f, -0.5f, -1.0f, $"{BOTTOM} 1"),
                            new(-1.0f, -0.5f, -1.0f, $"{BOTTOM} 2"),
                            new(-1.0f, -0.5f,  1.0f, $"{BOTTOM} 3"),
                            new( 1.0f, -0.5f,  1.0f, $"{BOTTOM} 4"),
                            new( 1.0f,  0.5f, -1.0f, $"{TOP} 1"),
                            new(-1.0f,  0.5f, -1.0f, $"{TOP} 2"),
                            new(-1.0f,  0.5f,  1.0f, $"{TOP} 3"),
                            new( 1.0f,  0.5f,  1.0f, $"{TOP} 4"),
                            new(Vector3.zero, CENTER)
                        });
                    break;
                case "Ashlands_Wall_2x2":
                case "Ashlands_Wall_2x2_top":
                case "Ashlands_Wall_2x2_edge2":
                case "Ashlands_Wall_2x2_edge2_top":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new NamedSnapPoint[]
                        {
                            new(-1.0f, 0.0f, -0.5f, $"{INNER} {BOTTOM} 1"),
                            new( 1.0f, 0.0f, -0.5f, $"{INNER} {BOTTOM} 2"),
                            new(-1.0f, 2.0f, -0.5f, $"{INNER} {TOP} 1"),
                            new( 1.0f, 2.0f, -0.5f, $"{INNER} {TOP} 2"),

                            new(-1.0f, 0.0f,  0.5f, $"{OUTER} {BOTTOM} 1"),
                            new( 1.0f, 0.0f,  0.5f, $"{OUTER} {BOTTOM} 2"),
                            new(-1.0f, 2.0f,  0.5f, $"{OUTER} {TOP} 1"),
                            new( 1.0f, 2.0f,  0.5f, $"{OUTER} {TOP} 2"),
                        });
                    break;
                case "Ashlands_Wall_2x2_edge":
                case "Ashlands_Wall_2x2_edge_top":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new NamedSnapPoint[]
                        {
                            // NOTE: Inner and Outer coordinates are inverted, as this piece seems 180 degrees rotated compared to similar pieces
                            new( 1.0f, 0.0f,  0.5f, $"{INNER} {BOTTOM} 1"),
                            new(-1.0f, 0.0f,  0.5f, $"{INNER} {BOTTOM} 2"),
                            new( 1.0f, 2.0f,  0.5f, $"{INNER} {TOP} 1"),
                            new(-1.0f, 2.0f,  0.5f, $"{INNER} {TOP} 2"),

                            new( 1.0f, 0.0f, -0.5f, $"{OUTER} {BOTTOM} 1"),
                            new(-1.0f, 0.0f, -0.5f, $"{OUTER} {BOTTOM} 2"),
                            new( 1.0f, 2.0f, -0.5f, $"{OUTER} {TOP} 1"),
                            new(-1.0f, 2.0f, -0.5f, $"{OUTER} {TOP} 2"),
                        });
                    break;
                case "Ashlands_Wall_2x2_cornerR":
                case "Ashlands_Wall_2x2_cornerR_top":
                    {
                        float xOffset = -0.25f;
                        SnapPointHelper.AddSnapPoints(
                            prefab,
                            new NamedSnapPoint[]
                            {
                                new(xOffset + 0.0f, 0.0f, -1.0f, $"{INNER} {BOTTOM} 1"),
                                new(xOffset + 0.0f, 0.0f,  0.0f, $"{INNER} {BOTTOM} {CORNER}"), // Also technically the BOTTOM CENTER
                                new(xOffset + 1.0f, 0.0f,  0.0f, $"{INNER} {BOTTOM} 2"),
                                new(xOffset + 0.0f, 2.0f, -1.0f, $"{INNER} {TOP} 1"),
                                new(xOffset + 0.0f, 2.0f,  0.0f, $"{INNER} {TOP} {CORNER}"), // Also technically the TOP CENTER
                                new(xOffset + 1.0f, 2.0f,  0.0f, $"{INNER} {TOP} 2"),

                                new(xOffset - 1.0f, 0.0f, -1.0f, $"{OUTER} {BOTTOM} 1"),
                                new(xOffset - 1.0f, 0.0f,  1.0f, $"{OUTER} {BOTTOM} {CORNER}"),
                                new(xOffset + 1.0f, 0.0f,  1.0f, $"{OUTER} {BOTTOM} 2"),
                                new(xOffset - 1.0f, 2.0f, -1.0f, $"{OUTER} {TOP} 1"),
                                new(xOffset - 1.0f, 2.0f,  1.0f, $"{OUTER} {TOP} {CORNER}"),
                                new(xOffset + 1.0f, 2.0f,  1.0f, $"{OUTER} {TOP} 2")
                            });
                        break;
                    }
                case "Ashlands_Wall_2x2_cornerL":
                case "Ashlands_Wall_2x2_cornerL_top":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new NamedSnapPoint[]
                        {
                            new( 0.0f,  0.0f, -1.25f, $"{INNER} {BOTTOM} 1"),
                            new( 0.0f,  0.0f, -0.5f, $"{INNER} {BOTTOM} {CORNER}"), // Also technically the BOTTOM CENTER
                            new( 1.25f, 0.0f, -0.5f, $"{INNER} {BOTTOM} 2"),
                            new( 0.0f,  2.0f, -1.25f, $"{INNER} {TOP} 1"),
                            new( 0.0f,  2.0f, -0.5f, $"{INNER} {TOP} {CORNER}"), // Also technically the TOP CENTER
                            new( 1.25f, 2.0f, -0.5f, $"{INNER} {TOP} 2"),

                            new(-1.0f,  0.0f, -1.25f, $"{OUTER} {BOTTOM} 1"),
                            new(-1.0f,  0.0f,  0.5f, $"{OUTER} {BOTTOM} {CORNER}"),
                            new( 1.25f, 0.0f,  0.5f, $"{OUTER} {BOTTOM} 2"),
                            new(-1.0f,  2.0f, -1.25f, $"{OUTER} {TOP} 1"),
                            new(-1.0f,  2.0f,  0.5f, $"{OUTER} {TOP} {CORNER}"),
                            new( 1.25f, 2.0f,  0.5f, $"{OUTER} {TOP} 2")
                        });
                    break;
                case "Ashlands_Fortress_Wall_Spikes":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new NamedSnapPoint[]
                        {
                            new(0.0f, 0.0f, -2.0f, $"{EDGE} 1"),
                            new(0.0f, 0.0f,  0.0f, CENTER),
                            new(0.0f, 0.0f,  2.0f, $"{EDGE} 2")
                        });
                    break;

                default:
                    break;
            }

            if (MorePrefabs.PatchPortalTexture && prefab.name == "portal" && !GUIManager.IsHeadless())
            {
                MeshRenderer meshRender = prefab.transform.Find("New").Find("model").GetComponent<MeshRenderer>();
                if (meshRender)
                {
                    meshRender.material.mainTexture = TextureHelper.GetBlackMarblePortalTexture();
                    meshRender.material.SetTexture(
                        TextureHelper.TextureNames.BumpMap,
                        TextureHelper.GetBlackMarblePortalBumpMap()
                    );
                }
            }

            if (MorePrefabs.IsEnableComfortPatches)
            {
                ApplyComfortPatches(prefab);
            }
        }

        /// <summary>
        ///     Applies patches to selected pieces so they grant
        ///     comfort as expected if they were vanilla pieces.
        /// </summary>
        /// <param name="prefab"></param>
        private static void ApplyComfortPatches(GameObject prefab)
        {
            if (prefab.TryGetComponent(out Piece piece))
            {
                switch (prefab.name)
                {
                    case "dvergrprops_chair":
                    case "dvergrprops_stool":
                        piece.m_comfortGroup = Piece.ComfortGroup.Chair;
                        piece.m_comfort = 2;
                        break;

                    case "mountainkit_chair":
                        piece.m_comfortGroup = Piece.ComfortGroup.Chair;
                        piece.m_comfort = 1;
                        break;

                    case "dvergrprops_bed":
                        piece.m_comfortGroup = Piece.ComfortGroup.Bed;
                        piece.m_comfort = 2;
                        break;

                    case "goblin_bed":
                        piece.m_comfortGroup = Piece.ComfortGroup.Bed;
                        piece.m_comfort = 1;
                        break;

                    case "ArmorStand_Female":
                    case "ArmorStand_Male":
                        piece.m_comfortGroup = Piece.ComfortGroup.None;
                        piece.m_comfort = 2;
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
