using HarmonyLib;
using MoreVanillaBuildPrefabs.Helpers;

namespace MoreVanillaBuildPrefabs.Patches
{
    [HarmonyPatch(typeof(WearNTear))]
    internal class WearNTearPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(WearNTear.Destroy))]
        private static void DestroyPrefix(WearNTear __instance, out EffectList __state)
        {
            var prefabName = __instance.name.RemoveSuffix("(Clone)");
            if (InitManager.IsPatchedByMod(prefabName))
            {
                if (!SfxHelper.HasSfx(__instance.m_destroyedEffect))
                {
                    __state = __instance.m_destroyedEffect;
                    __instance.m_destroyedEffect = SfxHelper.FixRemovalSfx(__instance);
                    return;
                }
            }
            __state = null;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(WearNTear.Destroy))]
        private static void DestroyPostfix(WearNTear __instance, EffectList __state)
        {
            if (__state != null)
            {
                __instance.m_destroyedEffect = __state;
            }
        }
    }
}