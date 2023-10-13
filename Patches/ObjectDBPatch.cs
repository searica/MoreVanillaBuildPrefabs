using HarmonyLib;
using MoreVanillaBuildPrefabs.Helpers;
using MoreVanillaBuildPrefabs.Logging;



namespace MoreVanillaBuildPrefabs.Patchess
{
    [HarmonyPatch(typeof(ObjectDB))]
    internal class ObjectDBPatch
    {
        // Hook here to add pieces
        [HarmonyPostfix]
        [HarmonyPriority(Priority.Low)]
        [HarmonyPatch(nameof(ObjectDB.Awake))]
        static void ObjectDBAwakePostfix()
        {
            Log.LogInfo("ObjectDBAwakePostfix()");

            HammerHelper.AddCustomCategories();
            MoreVanillaBuildPrefabs.InitPrefabRefs();
        }
    }
}
