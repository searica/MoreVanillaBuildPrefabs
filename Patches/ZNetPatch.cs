using HarmonyLib;
using MoreVanillaBuildPrefabs.Configs;
using MoreVanillaBuildPrefabs.Logging;
using UnityEngine.SceneManagement;

namespace MoreVanillaBuildPrefabs.Patches
{
    [HarmonyPatch(typeof(ZNet))]
    internal class ZNetPatch
    {
        /// <summary>
        ///     Patch to check if world modifiers for resources are active
        ///     and re-initialize the mod if they are so pickables have the
        ///     correct build requirement costs.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(nameof(ZNet.Start))]
        public static void ZNetStartPostfix()
        {
            if (PluginConfig.IsVerbosityMedium)
            {
                Log.LogInfo("Checking world modifiers");
            }

            if (SceneManager.GetActiveScene() == null)
            {
                return;
            }

            // If loading into game world and prefabs have not been added
            if (SceneManager.GetActiveScene().name == "main")
            {
                if (Game.m_resourceRate == 1.0f)
                {
                    return;
                }

                Log.LogInfo("World modifiers for resource rate are active, re-initializing");

                var watch = new System.Diagnostics.Stopwatch();
                if (PluginConfig.IsVerbosityMedium)
                {
                    watch.Start();
                }

                MoreVanillaBuildPrefabs.InitPieceRefs();
                MoreVanillaBuildPrefabs.InitPieces();
                MoreVanillaBuildPrefabs.InitHammer();

                if (PluginConfig.IsVerbosityMedium)
                {
                    watch.Stop();
                    Log.LogInfo($"Time to re-initialize: {watch.ElapsedMilliseconds} ms");
                }
            }
        }

        /// <summary>
        ///     Patch to save config file on log out to
        ///     ensure persistent settings on server.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(nameof(ZNet.Disconnect))]
        public static void ZNetDisconnectPrefix()
        {
            PluginConfig.Save();
        }
    }
}