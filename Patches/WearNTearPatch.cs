// Ignore Spelling: MVBP

using HarmonyLib;
using MVBP.Helpers;

namespace MVBP.Patches
{
    [HarmonyPatch(typeof(WearNTear))]
    internal class WearNTearPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(WearNTear.Destroy))]
        private static void DestroyPrefix(WearNTear __instance, out EffectList __state)
        {
            if (InitManager.IsPatchedByMod(__instance))
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