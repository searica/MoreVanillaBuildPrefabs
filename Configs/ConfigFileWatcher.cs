using System;
using System.IO;
using BepInEx.Configuration;
using BepInEx;
using Logging;


namespace Configs;
internal sealed class ConfigFileWatcher
{
    private const long RELOAD_DELAY = 10000000; // One second

    private DateTime lastReadTime = DateTime.MinValue;
    private readonly ConfigFile configFile;
    private readonly string ConfigFileDir;
    private readonly string ConfigFileName;

    /// <summary>
    ///     Create a file watcher to triger reloads of the config file when 
    ///     it is chaned, created, or renamed.
    /// </summary>
    /// <param name="configFile"></param>
    internal ConfigFileWatcher(ConfigFile configFile)
    {
        this.configFile = configFile;
        ConfigFileDir = Directory.GetParent(configFile.ConfigFilePath).FullName;
        ConfigFileName = Path.GetFileName(configFile.ConfigFilePath);
        var watcher = new FileSystemWatcher(ConfigFileDir, ConfigFileName);
        watcher.Changed += ReloadConfigFile;
        watcher.Created += ReloadConfigFile;
        watcher.Renamed += ReloadConfigFile;
        watcher.IncludeSubdirectories = true;
        watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
        watcher.EnableRaisingEvents = true;
    }

    /// <summary>
    ///     Event triggered after the file watcher reloads the configuration file.
    /// </summary>
    internal event Action OnConfigFileReloaded;

    /// <summary>
    ///     Safely invoke the <see cref="OnConfigFileReloaded"/> event
    /// </summary>
    private void InvokeOnConfigFileReloaded()
    {
        OnConfigFileReloaded?.SafeInvoke();
    }

    /// <summary>
    ///     Reloads config file if and only if the last write time difers from the last read time.
    /// </summary>
    /// <param name="configFile"></param>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    internal void ReloadConfigFile(object sender, FileSystemEventArgs eventArgs)
    {
        DateTime now = DateTime.Now;
        long deltaTime = now.Ticks - lastReadTime.Ticks;
        if (!File.Exists(configFile.ConfigFilePath) || deltaTime < RELOAD_DELAY)
        {
            return;
        }

        try
        {
            Log.LogInfo($"Reloading {configFile.ConfigFilePath}");
            bool saveOnConfigSet = configFile.DisableSaveOnConfigSet(); // turn off saving on config entry set
            configFile.Reload();
            configFile.SaveOnConfigSet = saveOnConfigSet; // reset config saving state
            lastReadTime = now;
            InvokeOnConfigFileReloaded(); // fire event
        }
        catch
        {
            Log.LogError($"There was an issue loading {configFile.ConfigFilePath}");
            Log.LogError("Please check your config entries for spelling and format!");
        }
    }
}
