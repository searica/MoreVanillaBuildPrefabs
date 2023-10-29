using BepInEx.Bootstrap;

namespace MVBP
{
    internal class ModCompat
    {
        public const string ExtraSnapPointsMadeEasyGUID = "Searica.Valheim.ExtraSnapPointsMadeEasy";
        public const string PlanBuildGUID = "marcopogo.PlanBuild";

        internal static bool IsPlanBuildInstalled => Chainloader.PluginInfos.ContainsKey(PlanBuildGUID);

        internal static bool IsExtraSnapPointsMadeEasyInstalled => Chainloader.PluginInfos.ContainsKey(ExtraSnapPointsMadeEasyGUID);
    }
}