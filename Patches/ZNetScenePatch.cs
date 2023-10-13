using HarmonyLib;

using MoreVanillaBuildPrefabs.Logging;
using static MoreVanillaBuildPrefabs.MoreVanillaBuildPrefabs;

namespace MoreVanillaBuildPrefabs
{

    [HarmonyPatch(typeof(ZNetScene))]
    internal class ZNetScenePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(ZNetScene.Awake))]
        public static void ZNetSceneAwakePostfix(ZNetScene __instance)
        {
            Log.LogInfo("ZNetSceneAwake");
            Log.LogInfo("Performing final mod initialization");
            FinalInit();
        }
    }
}
