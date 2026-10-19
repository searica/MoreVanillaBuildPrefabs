using HarmonyLib;
using Logging;

namespace MVBP.Extensions;
internal static class CodeMatchExtensions
{
    /// <summary>
    /// Runs a forward match without mutating anything when it fails. A failed
    /// <see cref="CodeMatcher.MatchStartForward"/> leaves Pos at -1, so any chained
    /// RemoveInstruction/Insert throws an out of range exception - and a throwing
    /// transpiler makes HarmonyX discard every patch on the target method, including
    /// other mods'. Bail out through this instead so a stolen anchor only costs us
    /// our own patch.
    /// </summary>
    public static bool TryMatchStartForward(this CodeMatcher matcher, string failureMessage, params CodeMatch[] matches)
    {
        matcher.MatchStartForward(matches);
        if (matcher.IsInvalid)
        {
            Log.LogWarning($"{failureMessage} Anchor instruction not found, it was likely consumed by another mod. Skipping this patch.");
            return false;
        }
        return true;
    }

    /// <summary>
    /// <see cref="TryMatchStartForward"/>, but leaving the position on the last matched
    /// instruction instead of the first.
    /// </summary>
    public static bool TryMatchEndForward(this CodeMatcher matcher, string failureMessage, params CodeMatch[] matches)
    {
        matcher.MatchEndForward(matches);
        if (matcher.IsInvalid)
        {
            Log.LogWarning($"{failureMessage} Anchor instruction not found, it was likely consumed by another mod. Skipping this patch.");
            return false;
        }
        return true;
    }
}
