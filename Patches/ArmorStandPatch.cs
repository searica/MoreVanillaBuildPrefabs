//using HarmonyLib;
//using UnityEngine;

//using MoreVanillaBuildPrefabs.Logging;

//namespace MoreVanillaBuildPrefabs.Patches
//{

//    [HarmonyPatch(typeof(ArmorStand))]
//    internal static class ArmorStandPatch
//    {
//        [HarmonyPrefix]
//        [HarmonyPatch(nameof(ArmorStand.Awake))]
//        static void AwakePrefix()
//        {
//            Log.LogInfo("ArmorStand.Awake()");
//        }

//        [HarmonyPrefix]
//        [HarmonyPatch(nameof(ArmorStand.UpdateAttach))]
//        static void UpdateAttachPrefix()
//        {
//            Log.LogInfo("ArmorStand.UpdateAttach()");
//        }

//        [HarmonyPrefix]
//        [HarmonyPatch(nameof(ArmorStand.UpdateVisual))]
//        static void UpdateVisualPrefix()
//        {
//            Log.LogInfo("ArmorStand.UpdateVisual()");
//        }

//        [HarmonyPostfix]
//        [HarmonyPatch(nameof(ArmorStand.SetVisualItem))]
//        static void SetVisualItemPostfix(ArmorStand __instance, ref int index, ref string itemName, ref int variant)
//        {
//            ArmorStand.ArmorStandSlot armorStandSlot = __instance.m_slots[index];
//            if (armorStandSlot.m_visualName != itemName
//                || armorStandSlot.m_visualVariant != variant
//                || armorStandSlot.m_visualName == "")
//            {
//                return;
//            }
//            GameObject itemPrefab = ObjectDB.instance.GetItemPrefab(itemName);
//            if (itemPrefab == null)
//            {
//                ZLog.LogWarning("Missing item prefab " + itemName);
//                return;
//            }
//            var itemPrefab2 = GameObject.Instantiate(itemPrefab);
//            itemPrefab2.transform.localScale = Vector3.one * 1.05f;

//            ItemDrop component = itemPrefab.GetComponent<ItemDrop>();
//            armorStandSlot.m_currentItemName = component.m_itemData.m_shared.m_name;
//            ItemDrop component2 = itemPrefab.GetComponent<ItemDrop>();

//            if (component2 != null)
//            {
//                if (component2.m_itemData.m_dropPrefab == null)
//                {
//                    component2.m_itemData.m_dropPrefab = itemPrefab2.gameObject;
//                }
//                __instance.m_visEquipment.SetItem(
//                    armorStandSlot.m_slot,
//                    component2.m_itemData.m_dropPrefab.name,
//                    armorStandSlot.m_visualVariant
//                );
//                __instance.UpdateSupports();
//                __instance.m_cloths = __instance.GetComponentsInChildren<Cloth>();
//            }
//        }
//    }
//}