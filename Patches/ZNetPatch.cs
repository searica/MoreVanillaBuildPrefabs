// Ignore Spelling: MVBP

using HarmonyLib;
using MVBP.Helpers;
using UnityEngine.SceneManagement;

namespace MVBP.Patches {
    [HarmonyPatch(typeof(ZNet))]
    internal static class ZNetPatch {
        /// <summary>
        ///     Patch to check if world modifiers for resources are active
        ///     and re-initialize the mod if they are so pickables have the
        ///     correct build requirement costs.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(nameof(ZNet.Start))]
        public static void ZNetStartPostfix() {
            Log.LogInfo("Checking world modifiers", LogLevel.Medium);

            // If loading into game world and prefabs have not been added
            if (SceneManager.GetActiveScene().name == "main") {
                if (Game.m_resourceRate == 1.0f) { return; }

                Log.LogInfo("World modifiers for resource rate are active, re-initializing");

                var watch = new System.Diagnostics.Stopwatch();
                if (Log.IsVerbosityMedium) { watch.Start(); }

                InitManager.UpdatePieces();

                if (Log.IsVerbosityMedium) {
                    watch.Stop();
                    Log.LogInfo($"Time to re-initialize: {watch.ElapsedMilliseconds} ms");
                }
            }
        }
    }
}