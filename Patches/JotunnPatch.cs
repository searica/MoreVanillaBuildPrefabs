using HarmonyLib;
using Jotunn.Managers;

namespace MoreVanillaBuildPrefabs.Patches
{
    [HarmonyPatch(typeof(SynchronizationManager))]
    internal class JotunnPatch
    {
        /// <summary>
        ///     Disable individual config events when about
        ///     to recieve config data from the server. This
        ///     prevents re-intializing 1000+ times if a lot
        ///     of config entries change at once.
        ///
        ///     A single re-initalization is run after syncing
        ///     datat with the server via the
        ///     SynchronizationManager.OnConfigurationSynchronized event.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPriority(Priority.Low)]
        [HarmonyPatch("ConfigRPC_OnClientReceive")]
        private static void ConfigRPC_OnClientReceive_Prefix()
        {
            MoreVanillaBuildPrefabs.DisableIndividualConfigEvents = true;
        }
    }
}