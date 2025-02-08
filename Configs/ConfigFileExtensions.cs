using BepInEx.Configuration;

namespace Configs;

/// <summary>
///     Extends ConfigFile with a convenience method to bind config entries with less boilerplate code 
///     and explicitly expose commonly used configuration manager attributes. Defaults synced argument 
///     to false because this is for mods not using Jotunn.
/// </summary>
public static class ConfigFileExtensions
{
    /// <summary>
    ///     Sets SaveOnConfigSet to false and returns
    ///     the Value prior to calling this method.
    /// </summary>
    /// <returns></returns>
    public static bool DisableSaveOnConfigSet(this ConfigFile configFile)
    {
        bool val = configFile.SaveOnConfigSet;
        configFile.SaveOnConfigSet = false;
        return val;
    }
}
