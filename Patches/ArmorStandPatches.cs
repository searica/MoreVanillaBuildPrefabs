using System.Collections.Generic;
using HarmonyLib;

using MagicaCloth2;
using Logging;
using MVBP.Extensions;

namespace MVBP.Patches;


/// <summary>
///     Patches to prevent attaching capes or objects with MagicaCloth to the fancy armorstands
///     as doing so triggers errors.
/// </summary>
[HarmonyPatch]
internal static class ArmorStandPatches
{
    public const int ArmorStandMaleHash = 639316453;    // ArmorStand_Male
    public const int ArmorStandFemaleHash = 826840908;  // ArmorStand_Female

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ArmorStand), nameof(ArmorStand.CanAttach))]
    static void CanAttachPostfix(ArmorStand __instance, ItemDrop.ItemData item, ref bool __result)
    {
        if (__result && !CanAttachItemToArmorStand(__instance, item))
        {
            __result = false;
        }
    }

    public static bool CanAttachItemToArmorStand(ArmorStand armorStand, ItemDrop.ItemData item)
    {
        int prefabHash = armorStand.m_nview.m_zdo.m_prefab;

        if (prefabHash != ArmorStandMaleHash && prefabHash != ArmorStandFemaleHash)
        {
            return true;
        }

        if (item.m_dropPrefab && item.m_dropPrefab.TryGetComponentInChildren(out MagicaCloth2.MagicaCloth _, includeInactive: true))
        {
            Log.LogInfo($"Cannot attach item {item.m_dropPrefab.name} to ArmorStand due to MagicaCloth incompatibility.");
            return false;
        }

        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.SetupCloth))]
    static void SetupClothPrefix(VisEquipment __instance, ref List<ColliderComponent> __state)
    {
        if (__instance.m_isArmorStand)
        {
            __state = __instance.m_clothColliders;
            __instance.m_clothColliders = [];
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.SetupCloth))]
    static void SetupClothPostfix(VisEquipment __instance, List<ColliderComponent> __state)
    {
        if (__instance.m_isArmorStand)
        {
            __instance.m_clothColliders = __state;
        }
    }
}



