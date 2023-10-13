using HarmonyLib;

using MoreVanillaBuildPrefabs.Logging;
using static MoreVanillaBuildPrefabs.MoreVanillaBuildPrefabs;

namespace MoreVanillaBuildPrefabs
{

    [HarmonyPatch(typeof(ZNetScene))]
    internal class ZNetScenePatch
    {

        [HarmonyPatch(typeof(ZNetScene), "Awake")]
        public static class ZNetSceneAwake
        {
            public static void Postfix(ZNetScene __instance)
            {
                Log.LogInfo("ZNetSceneAwake");
                Log.LogInfo("Performing final mod initialization");
                FinalInit();
            }
        }
    }
}
