using MVBP.Extensions;
using UnityEngine;

namespace MVBP.PrefabManagement;

/// <summary>
///  Applies edits to player built pieces.
/// </summary>
internal static class PlayerPiecePatcher
{
    private static readonly int PieceLayer = LayerMask.NameToLayer("piece");
    private static readonly int CharacterTriggerLayer = LayerMask.NameToLayer("character_trigger");
    private const float timeout = 1e30f;

    /// <summary>
    ///     Apply patches to player built pieces.
    ///     Called after Piece.Awake and Piece.SetCreator.
    /// </summary>
    /// <param name="piece"></param>
    internal static void PatchPlayerBuiltPieceIfNeeded(Piece piece)
    {
        if (!piece || !piece.gameObject || !piece.IsPlacedByPlayer() || !ZNetPrefabManager.IsPatchedByMVBP(piece))
        {
            return;
        }
        string prefabName = piece.gameObject.GetPrefabName();

        ApplyDoorPatches(prefabName, piece.gameObject);
        ApplyTimedDestructionPatch(piece.gameObject);
        ApplyContainerPatches(prefabName, piece.gameObject);

        if (piece.TryGetComponent(out Destructible destructible))
        {
            EditDestructibleSpawn(prefabName, destructible);
        }
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
            ApplyNewDvergrTexture(prefabName, piece.gameObject);
        }
    }

    /// <summary>
    ///     Modifies container size based on settings in default PrefabDB
    /// </summary>
    /// <param name="prefabName"></param>
    /// <param name="gameObject"></param>
    private static void ApplyContainerPatches(string prefabName, GameObject gameObject)
    {
        Container container = gameObject.GetComponentInChildren<Container>();
        if (!container)
        {
            return;
        }

        ZDO zdo = container.m_nview.GetZDO();
        if (zdo == null)
        {
            return;
        }

        // Check for wards for player built containers
        Piece piece = gameObject.GetComponentInChildren<Piece>();
        if (piece && piece.IsPlacedByPlayer())
        {
            zdo.Set("HasFields", true);
            zdo.Set("HasFieldsContainer", true);
            zdo.Set("Container.m_checkGuardStone", true);
        }

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


    private static void ApplyTimedDestructionPatch(GameObject gameObject)
    {
        if (!gameObject.TryGetComponent(out TimedDestruction timedDestruction))
        {
            return;
        }

        ZDO zdo = timedDestruction.m_nview.GetZDO();
        if (zdo == null)
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
    private static void EditDestructibleSpawn(string name, Destructible destructible)
    {
        if (!destructible || !PrefabConfigManager.TryGetPrefabConfig(name, out var prefabConfig, checkIfBound: true))
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

        ZDO zdo = destructible.m_nview.GetZDO();
        if (zdo == null)
        {
            return;
        }
        zdo.Set("HasFields", true);
        zdo.Set("HasFieldsDestructible", true);
        zdo.Set("Destructible.m_spawnWhenDestroyed", "fx_crystal_destruction");

        destructible.m_spawnWhenDestroyed = spawn;
    }



    /// <summary>
    ///     Sets the texture of certain dvergr pieces to use
    ///     a cleaner texture when they are above 50% health
    /// </summary>
    /// <param name="name"></param>
    /// <param name="gameObject"></param>
    private static void ApplyNewDvergrTexture(string name, GameObject gameObject)
    {
        if (PrefabConfigManager.DvergrWoodPieces.Contains(name))
        {
            Renderer[] componentsInChildren = gameObject.transform.Find("New").GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in componentsInChildren)
            {
                renderer.material.mainTexture = TextureHelper.GetNewDvergrTexture();
            }
        }
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

    private static void ApplyDoorPatches(string name, GameObject gameObject)
    {
        // Missing animations
        // dungeon_queen_door
        // dvergrtown_secretdoor

        switch (name)
        {
            case "dvergrtown_slidingdoor":
            case "dvergrtown_secretdoor":
                {
                    if (!gameObject.TryGetComponent(out Door door))
                    {
                        return;
                    }

                    door.m_canNotBeClosed = false;
                    door.m_checkGuardStone = true;

                    if (gameObject.TryGetComponent(out ZNetView nview))
                    {
                        ZDO zdo = nview.GetZDO();
                        if (zdo == null) { return; }
                        zdo.Set("HasFields", true);
                        zdo.Set("HasFieldsDoor", true);
                        zdo.Set("Door.m_canNotBeClosed", false);
                        zdo.Set("Door.m_checkGuardStone", true);
                    }
                }
                break;

            default:
                break;
        }
    }

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

    private static void AddBed(GameObject gameObject, Vector3 spawnPosition)
    {
        var attachPoint = new GameObject("spawnpoint");
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
