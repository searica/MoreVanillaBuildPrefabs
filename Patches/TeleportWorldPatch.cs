using System.Collections.Generic;
using HarmonyLib;
using System.Reflection.Emit;
using UnityEngine;
using MVBP.Helpers;

namespace MVBP.Patches
{
    [HarmonyPatch(typeof(TeleportWorld))]
    internal class TeleportWorldPatch
    {
        private static string PrefabName;

        [HarmonyPrefix]
        [HarmonyPatch(nameof(TeleportWorld.UpdatePortal))]
        private static void UpdatePortalPrefix(TeleportWorld __instance)
        {
            PrefabName = InitManager.GetPrefabName(__instance);
        }

        [HarmonyTranspiler]
        [HarmonyPatch(nameof(TeleportWorld.UpdatePortal))]
        private static IEnumerable<CodeInstruction> UpdatePortalTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            /* m_hadTarget = flag;
             * IL_006e: ldarg.0
             * IL_006f: ldloc.1
             * IL_0070: stfld bool TeleportWorld::m_hadTarget
             *
             * m_target_found.SetActive((bool)closestPlayer && closestPlayer.IsTeleportable() && TargetFound());
             * IL_0075: ldarg.0
             * IL_0076: ldfld class EffectFade TeleportWorld::m_target_found
             * IL_007b: ldloc.0
             * IL_007c: call bool [UnityEngine.CoreModule]UnityEngine.Object::op_Implicit(class [UnityEngine.CoreModule]UnityEngine.Object)
             * IL_0081: brfalse.s IL_0093
             * IL_0083: ldloc.0
             * IL_0084: callvirt instance bool Humanoid::IsTeleportable
             *
             * (no C# code)
             * IL_0089: brfalse.s IL_0093
             * IL_008b: ldarg.0
             * IL_008c: call instance bool TeleportWorld::TargetFound()
             * IL_0091: br.s IL_0094
             * IL_0093: ldc.i4.0
             * IL_0094: callvirt instance void EffectFade::SetActive(bool)
             * IL_0099: ret
             */

            return new CodeMatcher(instructions)
                .MatchForward(
                    useEnd: false,
                    new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Humanoid), nameof(Humanoid.IsTeleportable)))
                )
                .SetInstructionAndAdvance(Transpilers.EmitDelegate(IsTeleportable_Delegate))
                .InstructionEnumeration();
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(TeleportWorld.Teleport))]
        private static void TeleportPrefix(TeleportWorld __instance)
        {
            PrefabName = InitManager.GetPrefabName(__instance);
        }

        [HarmonyTranspiler]
        [HarmonyPatch(nameof(TeleportWorld.Teleport))]
        private static IEnumerable<CodeInstruction> TeleportWorldTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            /* if (!player.IsTeleportable())
             * IL_0066: ret
             * IL_0067: ldarg.1
             * IL_0068: callvirt instance bool Humanoid::IsTeleportable()
             * IL_006d: brtrue.s IL_007e
             *
             * player.Message(MessageHud.MessageType.Center, "$msg_noteleport");
             * IL_006f: ldarg.1
             * IL_0070: ldc.i4.2
             * IL_0071: ldstr "$msg_noteleport"
             * IL_0076: ldc.i4.0
             * IL_0077: ldnull
             * IL_0078: callvirt instance void Character::Message(valuetype MessageHud/MessageType, string, int32, class [UnityEngine.CoreModule]UnityEngine.Sprite)
             */
            return new CodeMatcher(instructions)
                .MatchForward(
                    useEnd: false,
                    new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Humanoid), nameof(Humanoid.IsTeleportable)))
                )
                .SetInstructionAndAdvance(Transpilers.EmitDelegate(IsTeleportable_Delegate))
                .InstructionEnumeration();
        }

        private static bool IsTeleportable_Delegate(Player player)
        {
            if (!string.IsNullOrEmpty(PrefabName) && PrefabName == "portal")
            {
                PrefabName = null;
                return true;
            }
            PrefabName = null;
            return player.IsTeleportable();
        }
    }
}