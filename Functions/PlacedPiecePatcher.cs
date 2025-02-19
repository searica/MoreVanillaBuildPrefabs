using HarmonyLib;
using Jotunn.Managers;
using MVBP.Extensions;
using MVBP.PieceManagement;
using MVBP.PrefabManagement;
using System.Collections.Generic;
using UnityEngine;

namespace MVBP.Functions;

[HarmonyPatch]
/// <summary>
///  Applies edits to player built pieces.
/// </summary>
internal static class PlacedPiecePatcher
{
    private static readonly int PieceLayer = LayerMask.NameToLayer("piece");
    private static readonly int CharacterTriggerLayer = LayerMask.NameToLayer("character_trigger");
    private const float timeout = 1e30f;


    /// <summary>
    ///     Applies patches from when pieces are loaded.
    /// </summary>
    /// <param name="__instance"></param>
    [HarmonyPostfix]
    [HarmonyPriority(Priority.VeryHigh)]
    [HarmonyPatch(typeof(Piece), nameof(Piece.Awake))]
    private static void PieceAwakePostfix(Piece __instance)
    {
        ApplyPlayerBuiltPiecePatches(__instance);
    }

    /// <summary>
    ///     Applies patches when pieces are first placed.
    /// </summary>
    /// <param name="__instance"></param>
    [HarmonyPostfix]
    [HarmonyPriority(Priority.VeryHigh)]
    [HarmonyPatch(typeof(Piece), nameof(Piece.SetCreator))]
    private static void PieceSetCreatorPostfix(Piece __instance)
    {
        ApplyPlayerBuiltPiecePatches(__instance);
    }

    /// <summary>
    ///     Apply patches to player built pieces.
    ///     Called after Piece.Awake and Piece.SetCreator.
    /// </summary>
    /// <param name="piece"></param>
    private static void ApplyPlayerBuiltPiecePatches(Piece piece)
    {
        if (!piece || !piece.gameObject || !piece.IsPlacedByPlayer())
        {
            return;  // invalid piece or non-player piece.
        }

        string prefabName = piece.gameObject.GetPrefabName();
        if (!ZNetPrefabManager.IsPatchedByMVBP(prefabName))
        {
            return;  // not patched by MVBP don't touch it.
        }


        // Make player-built remove-able pieces removeable
        // Have to do this after placement to avoid affecting non-player built instances.
        if (!ZNetPrefabManager.IsNonRemovablePiece(piece))
        {
            piece.m_canBeRemoved = true;
        }

        ApplyDvergrPortalPatches(prefabName, piece);
        ApplyDoorPatches(prefabName, piece);
        ApplyTimedDestructionPatch(piece);
        ApplyContainerPatches(prefabName, piece);
        EditDestructibleSpawn(prefabName, piece);

        if (MorePrefabs.IsEnablePlayerBasePatches)
        {
            ApplyPlayerBasePatches(prefabName, piece.gameObject);
        }
        if (MorePrefabs.IsEnableBedPatches)
        {
            ApplyBedPatches(prefabName, piece.gameObject);
        }
        if (MorePrefabs.IsEnableFermenterPatches)
        {
            ApplyFermenterPatches(prefabName, piece.gameObject);
        }
        if (MorePrefabs.PatchDvergrWoodTexture)
        {
            TextureManager.ApplyNewDvergrTexture(prefabName, piece.gameObject);
        }
        if (MorePrefabs.PatchPortalTexture)
        {
            TextureManager.ApplyPortalTexturePatch(prefabName, piece.gameObject);
        }
    }

    private static bool TryGetZDO(Piece piece, out ZDO zdo)
    {
        if (!piece.m_nview || !piece.m_nview.IsValid())
        {
            zdo = null;
            return false;
        }
        zdo = piece.m_nview.GetZDO();
        return zdo != null;
    }

    /// <summary>
    ///     Patch "portal" to save changes made by PrefabPatcher to zdo
    ///     for player build pieces. (see PrefabPatcher.PatchDvergrPortal)
    /// </summary>
    /// <param name="prefabName"></param>
    /// <param name="gameObject"></param>
    private static void ApplyDvergrPortalPatches(string prefabName, Piece piece)
    {
        if (prefabName != "portal")
        {
            return;
        }

        if (!TryGetZDO(piece, out ZDO zdo))
        {
            return;
        }

        zdo.Set("HasFields", true);
        zdo.Set("HasTeleportWorld", true);
        zdo.Set("TeleportWorld.m_allowAlItems", true);

        zdo.Set("HasPiece", true);
        zdo.Set("Piece.m_description", "$piece_portal_stone_description");
    }


    /// <summary>
    ///     Sets chest to check for wards and modifies container 
    ///     size based on settings in in the PrefabConfig for this prefab.
    /// </summary>
    /// <param name="prefabName"></param>
    /// <param name="gameObject"></param>
    private static void ApplyContainerPatches(string prefabName, Piece piece)
    {
        Container container = piece.gameObject.GetComponentInChildren<Container>();
        if (!container)
        {
            return;
        }

        if (!TryGetZDO(piece, out ZDO zdo))
        {
            return;
        }

        // Check for wards for player built containers
        zdo.Set("HasFields", true);
        zdo.Set("HasFieldsContainer", true);
        zdo.Set("Container.m_checkGuardStone", true);


        // Modify container size based on configs
        if (!PrefabConfigManager.TryGetPrefabConfig(prefabName, out var prefabConfig, checkIfBound: true))
        {
            return;
        }

        if (!prefabConfig.InvWidth.HasValue || !prefabConfig.InvHeight.HasValue)
        {
            return;
        }

        Inventory inventory = container.GetInventory();
        if (inventory == null)
        {
            return;
        }

        int width = (int)prefabConfig.InvWidth.Value;
        int height = (int)prefabConfig.InvHeight.Value;

        zdo.Set("HasFields", true);
        zdo.Set("HasFieldsContainer", true);
        zdo.Set("Container.m_width", width);
        zdo.Set("Container.m_height", height);

        zdo.Set("HasFieldsInventory", true);
        zdo.Set("Inventory.m_width", width);
        zdo.Set("Inventory.m_height", height);

        container.m_width = width;
        container.m_height = height;
        inventory.m_width = width;
        inventory.m_height = height;
    }


    private static void ApplyTimedDestructionPatch(Piece piece)
    {
        if (!piece.gameObject.TryGetComponent(out TimedDestruction timedDestruction))
        {
            return;
        }

        if (!TryGetZDO(piece, out ZDO zdo))
        {
            return;
        }

        zdo.Set("HasFields", true);
        zdo.Set("HasFieldsTimedDestruction", true);
        zdo.Set("TimedDestruction.m_timeout", timeout);
        timedDestruction.m_timeout = timeout;
    }

    /// <summary>
    ///     Edit the prefab and ZDO to change the value of m_spawnOnDestroyed in a
    ///     persistent way based on the default config settings.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="destructible"></param>
    private static void EditDestructibleSpawn(string name, Piece piece)
    {
        if (!piece.TryGetComponent(out Destructible destructible) || !destructible)
        {
            return;
        }

        if (!TryGetZDO(piece, out ZDO zdo))
        {
            return;
        }

        if (!PrefabConfigManager.TryGetPrefabConfig(name, out var prefabConfig, checkIfBound: true))
        {
            return;
        }

        if (string.IsNullOrEmpty(prefabConfig.SpawnOnDestroyed))
        {
            return;
        }

        GameObject spawn = ZNetScene.instance.GetPrefab(prefabConfig.SpawnOnDestroyed);
        if (!spawn)
        {
            return;
        }

        zdo.Set("HasFields", true);
        zdo.Set("HasFieldsDestructible", true);
        zdo.Set("Destructible.m_spawnWhenDestroyed", prefabConfig.SpawnOnDestroyed);
        destructible.m_spawnWhenDestroyed = spawn;
    }

    /// <summary>
    ///     Adds PlayerBase effect to pieces based on PieceDB settings
    /// </summary>
    /// <param name="name"></param>
    /// <param name="gameObject"></param>
    private static void ApplyPlayerBasePatches(string name, GameObject gameObject)
    {
        if (PrefabConfigManager.TryGetPrefabConfig(name, out PrefabConfig prefabConfig, checkIfBound: true))
        {
            if (prefabConfig.PlayerBasePatch)
            {
                AddPlayerBase(gameObject);
            }
        }
    }

    // TODO: can I add game objects via ZDO or just components?
    private static void AddPlayerBase(GameObject gameObject)
    {
        // create PlayerBase object as child
        var playerBase = new GameObject("PlayerBase");
        playerBase.transform.parent = gameObject.transform;
        playerBase.transform.localScale = Vector3.one;
        playerBase.transform.localPosition = Vector3.zero;
        playerBase.layer = CharacterTriggerLayer;

        // Set up character trigger sphere collider
        SphereCollider collider = playerBase.AddComponent<SphereCollider>();
        collider.center = Vector3.zero;
        collider.radius = 20;
        collider.enabled = true;
        collider.isTrigger = true;

        // Add player base type EffectArea
        EffectArea playerBaseEffect = playerBase.AddComponent<EffectArea>();
        playerBaseEffect.enabled = true;
        playerBaseEffect.m_type = EffectArea.Type.PlayerBase;
    }


    /// <summary>
    ///     Edit fields and zdo to make door closable after opening.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="gameObject"></param>
    private static void ApplyDoorPatches(string name, Piece piece)
    {
        // Missing animations
        // dungeon_queen_door
        // dvergrtown_secretdoor

        switch (name)
        {
            case "dvergrtown_slidingdoor":
            case "dvergrtown_secretdoor":
                {

                    if (!piece.TryGetComponent(out Door door))
                    {
                        return;
                    }
                    if (!TryGetZDO(piece, out ZDO zdo))
                    {
                        return;
                    }
                    door.m_canNotBeClosed = false;
                    door.m_checkGuardStone = true;
                    zdo.Set("HasFields", true);
                    zdo.Set("HasFieldsDoor", true);
                    zdo.Set("Door.m_canNotBeClosed", false);
                    zdo.Set("Door.m_checkGuardStone", true);
                }
                break;

            default:
                break;
        }
    }


    /// <summary>
    ///     Adds bed to select prefabs.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="gameObject"></param>
    private static void ApplyBedPatches(string name, GameObject gameObject)
    {
        switch (name)
        {
            case "goblin_bed":
                AddBed(gameObject, new Vector3(0, 0.45f, 0));
                break;

            case "dvergrprops_bed":
                AddBed(gameObject, new Vector3(0, 0.45f, 0));
                break;

            default:
                break;
        }
    }

    /// <summary>
    ///     Add bed component and set spawn point attach point.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="spawnPosition"></param>
    private static void AddBed(GameObject gameObject, Vector3 spawnPosition)
    {
        GameObject attachPoint = new("spawnpoint");
        attachPoint.transform.parent = gameObject.transform;
        attachPoint.transform.localPosition = spawnPosition;
        attachPoint.layer = PieceLayer;

        Bed bed = gameObject.AddComponent<Bed>();
        bed.m_spawnPoint = attachPoint.transform;

        // TODO: can I add game objects via ZDO or just components?
    }

    /// <summary>
    ///     Applies patches to selected pieces so they
    ///     function as a fermenter
    /// </summary>
    /// <param name="prefab"></param>
    private static void ApplyFermenterPatches(string name, GameObject gameObject)
    {
        switch (name)
        {
            case "dvergrprops_barrel":

                // Get child object clones
                GameObject FermenterPrefab = ZNetScene.instance?.GetPrefab("fermenter");
                GameObject add_button = FermenterPrefab.transform.Find("add_button").gameObject.DeepCopy();
                GameObject tap_button = FermenterPrefab.transform.Find("tap_button").gameObject.DeepCopy();
                GameObject roofcheckpoint = FermenterPrefab.transform.Find("roofcheckpoint").gameObject.DeepCopy();
                GameObject output = FermenterPrefab.transform.Find("output").gameObject.DeepCopy();
                GameObject _ready = FermenterPrefab.transform.Find("_ready").gameObject.DeepCopy();
                GameObject _fermenting = FermenterPrefab.transform.Find("_fermenting").gameObject.DeepCopy();

                Fermenter VanillaFermenter = FermenterPrefab.GetComponent<Fermenter>();

                // Assign to dvergrprops barrel
                add_button.transform.parent = gameObject.transform;
                tap_button.transform.parent = gameObject.transform;
                roofcheckpoint.transform.parent = gameObject.transform;
                output.transform.parent = gameObject.transform;
                _ready.transform.parent = gameObject.transform;
                _fermenting.transform.parent = gameObject.transform;

                // Adjust locations
                add_button.transform.localScale = Vector3.one;
                add_button.transform.localPosition = new Vector3(0f, 0.75f, 0f);

                tap_button.transform.localPosition = new Vector3(0f, 0.5f, 0.9f);
                output.transform.localPosition = new Vector3(0f, 0.5f, 1.2f);
                roofcheckpoint.transform.localPosition = new Vector3(0f, 1.5f, 0f);
                _ready.transform.localPosition = new Vector3(0f, 0.75f, 0f);
                _fermenting.transform.localPosition = new Vector3(0f, 0.75f, 0f);

                // make fake top
                var _top = new GameObject("_top");
                _top.transform.parent = gameObject.transform;

                // Set up fermenter component
                bool activeState = gameObject.activeSelf;
                gameObject.SetActive(false);
                Fermenter fermenter = gameObject.AddComponent<Fermenter>();

                fermenter.m_addSwitch = add_button.GetComponent<Switch>();
                fermenter.m_tapSwitch = tap_button.GetComponent<Switch>();
                fermenter.m_roofCheckPoint = roofcheckpoint.transform;
                fermenter.m_topObject = _top;
                fermenter.m_readyObject = _ready;
                fermenter.m_fermentingObject = _fermenting;
                fermenter.m_outputPoint = output.transform;

                // Copy values from vanilla fermenter
                fermenter.m_tapDelay = VanillaFermenter.m_tapDelay;
                fermenter.m_updateCoverTimer = VanillaFermenter.m_updateCoverTimer;
                fermenter.m_fermentationDuration = VanillaFermenter.m_fermentationDuration * 0.7f;
                fermenter.m_name = VanillaFermenter.m_name;

                // fix effects
                fermenter.m_addedEffects = VanillaFermenter.m_addedEffects;
                fermenter.m_tapEffects = VanillaFermenter.m_tapEffects;
                fermenter.m_spawnEffects = VanillaFermenter.m_spawnEffects;
                fermenter.m_conversion = VanillaFermenter.m_conversion;

                gameObject.SetActive(activeState);

                AddPlayerBase(gameObject);
                break;

            default:
                break;
        }
    }
}
