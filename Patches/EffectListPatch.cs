﻿// Ignore Spelling: MVBP

using HarmonyLib;

namespace MVBP.Patches
{
    [HarmonyPatch(typeof(EffectList))]
    internal static class EffectListPatch
    {
        /// <summary>
        ///     Patch to fix null reference error when using ArmorStand_Male or ArmorStand_Female.
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPrefix]
        [HarmonyPatch(nameof(EffectList.Create))]
        private static void CreatePrefix(ref EffectList __instance)
        {
            foreach (EffectList.EffectData effectData in __instance.m_effectPrefabs)
            {
                if (effectData != null && effectData.m_enabled && !effectData.m_prefab)
                {
                    effectData.m_enabled = false;
                }
            }
        }
    }
}