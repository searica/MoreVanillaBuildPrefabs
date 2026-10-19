// Ignore Spelling: MVBP

using HarmonyLib;
using Logging;
using MVBP.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace MVBP.Patches;

[HarmonyPatch(typeof(MineRock))]
internal static class MineRockPatch
{
    /// <summary>
    ///     Prevent MineRock from updating visibility until
    ///     after it acquires a ZNetView (such as by restarting
    ///     the game or hitting it with something).
    /// </summary>
    /// <param name="__instance"></param>
    /// <returns></returns>
    [HarmonyPrefix]
    [HarmonyPatch(nameof(MineRock.UpdateVisability))]
    private static bool UpdateVisabilityPrefix(MineRock __instance)
    {
        Log.LogInfo("MineRock.UpdateVisability patch applied", Log.InfoLevel.High);
        // Try to avoid accessing value from null object?
        return __instance && __instance.m_nview != null;
    }

    ///// <summary>
    /////     HitData stores the ZDOID of the attcker yet MineRock.RPC_Hit calls GetAttacker().GetZDOID() instead
    /////     of checking m_attacker to get the ZDO directly, which can cause NRE and is inefficent.
    ///// </summary>
    ///// <param name="instructions"></param>
    ///// <returns></returns>
    //[HarmonyTranspiler]
    //private static IEnumerable<CodeInstruction> FixGetAttackerNRE(IEnumerable<CodeInstruction> instructions)
    //{
    //    var codeMatches = new CodeMatch[]
    //    {
    //        new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(HitData), nameof(HitData.GetAttacker))),
    //        new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Character), nameof(Character.GetZDOID)))
    //    };

    //    // want to replace calls to hit.GetAttacked().GetZDOID() with calls to hit.m_attacker
    //    CodeMatcher matcher = new CodeMatcher(instructions);
    //    matcher.TryMatchStartForward()
    //    return new CodeMatcher(instructions)
    //        .MatchForward(useEnd: false, codeMatches)
    //        .SetInstructionAndAdvance(Transpilers.EmitDelegate(SetupPlacementGhostInstantiateDelegate))
    //        .InstructionEnumeration();
    //}
}
