using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx;
using Jotunn.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Configs;

/// <summary>
///     Custom drawer for Jotunn RequirementConfigs and associated helpers.
/// </summary>
internal static class ReqConfigDrawer
{
    private static readonly List<string> ConfigManagerGUIDs = new List<string>()
    {
        "_shudnal.ConfigurationManager",
        "com.bepis.bepinex.configurationmanager"
    };

    private static BaseUnityPlugin _configManager = null;

    private static BaseUnityPlugin GetConfigManager()
    {
        foreach (string GUID in ConfigManagerGUIDs)
        {
            if (Chainloader.PluginInfos.TryGetValue(GUID, out PluginInfo configManagerInfo) && configManagerInfo.Instance)
            {
                return configManagerInfo.Instance;
            }
        }
        return null;
    }

    private static BaseUnityPlugin ConfigManager => _configManager ??= GetConfigManager();

    private const int GutterWidth = 12;
    private const int AmountWidth = 33;
    private const int UpgradeWidth = 37;
    private const int ButtonWidth = 21;


    /// <summary>
    ///     Custom drawer for RequirementConfigs.
    /// </summary>
    /// <param name="amountSep">Char use to separate amounts.</param>
    /// <param name="reqSep">Char used to separate requirements.</param>
    /// <param name="hasUpgrades">Whether the item can be upgraded and there is an amount per level value.</param>
    /// <returns></returns>
    public static Action<ConfigEntryBase> ReqConfigCustomDrawer(char amountSep = ',', char reqSep = ';', bool hasUpgrades = false)
    {
        return cfg =>
        {
            int RightColumnWidth = GetRightColumnWidth();
            int prefabNameWidth = RightColumnWidth - AmountWidth - (hasUpgrades ? UpgradeWidth : 0) - ButtonWidth * 2 - GutterWidth;

            GUIStyle amountStyle = new(GUI.skin.textField) { fixedWidth = AmountWidth };
            GUIStyle buttonStyle = new(GUI.skin.button) { fixedWidth = ButtonWidth };
            GUIStyle prefabStyle = new(GUI.skin.textField) { fixedWidth = prefabNameWidth };

            RequirementsParser reqParser = new(amountSep, reqSep);
            List<RequirementConfig> newReqs = [];      
            List<RequirementConfig> reqs = reqParser.Deserialize((string)cfg.BoxedValue);

            bool wasUpdated = false;
            GUILayout.BeginVertical();
            foreach (RequirementConfig req in reqs)
            {
                GUILayout.BeginHorizontal();
                string newItem = GUILayout.TextField(req.Item, prefabStyle);
                string prefabName = string.IsNullOrEmpty(newItem) ? req.Item : newItem;
                wasUpdated = wasUpdated || prefabName != req.Item;

                int amount = req.Amount;
                if (int.TryParse(GUILayout.TextField(amount.ToString(), amountStyle), out int newAmount) && newAmount != amount)
                {
                    amount = newAmount;
                    wasUpdated = true;
                }

                if (GUILayout.Button("x", buttonStyle))
                {
                    wasUpdated = true;  // this requirement was removed
                }
                else
                {
                    newReqs.Add(new RequirementConfig { Item = prefabName, Amount = amount });
                }

                if (GUILayout.Button("+", buttonStyle))
                {
                    wasUpdated = true;
                    newReqs.Add(RequirementsParser.GetPlaceholderReqConfig());
                }

                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            if (wasUpdated)
            {
                cfg.BoxedValue = reqParser.Serialize(newReqs);
            }
        };
    }

    internal static int GetRightColumnWidth()
    {
        int result = 130;
        if (ConfigManager != null)
        {
            PropertyInfo pi = ConfigManager?.GetType().GetProperty("RightColumnWidth", BindingFlags.Instance | BindingFlags.NonPublic);
            if (pi != null)
            {
                result = (int)pi.GetValue(ConfigManager);
            }
        }

        return result;
    }

    internal static void ReloadConfigDisplay()
    {

        if (ConfigManager != null && ConfigManager.GetType()?.GetProperty("DisplayingWindow")?.GetValue(ConfigManager) is true)
        {
            ConfigManager.GetType().GetMethod("BuildSettingList").Invoke(ConfigManager, Array.Empty<object>());
        }
    }

    /// <summary>
    ///     Acceptable values for config entries using ReqConfigDrawer as a custom drawer.
    /// </summary>
    public class AcceptableValueReqConfigNote : AcceptableValueBase
    {
        public virtual string Note { get; }

        /// <summary>
        ///     Allows setting a description of acceptable values for a config entry using . 
        ///     For example if the config entry use CustomDrawer = ReqConfigDrawer and
        ///     lets users configure the requirements to build a piece, the note should 
        ///     detail what values are valid: "You must use valid spawn item codes."
        /// </summary>
        /// <param name="note">The note displayed to users describing what acceptable values are.</param>
        /// <exception cref="ArgumentException"></exception>
        public AcceptableValueReqConfigNote(string note = "You must use valid spawn item codes.") : base(typeof(string))
        {
            if (string.IsNullOrEmpty(note))
            {
                throw new ArgumentException("A string with atleast 1 character is needed", "Note");
            }
            this.Note = note;
        }

        // passthrough overrides
        public override object Clamp(object value) { return value; }
        public override bool IsValid(object value) { return !string.IsNullOrEmpty(value as string); }

        public override string ToDescriptionString()
        {
            return "# Note: " + Note;
        }
    }

    /// <summary>
    ///     Class to handle serializing and de-serializing piece requirements.
    /// </summary>
    public class RequirementsParser
    {
        public const string PrefabNamePlaceholder = "<PrefabName>";
        private readonly char reqSep;
        private readonly char amountSep;

        /// <summary>
        ///     Create a RequirementsParser instance with specified separators.
        /// </summary>
        /// <param name="amountSep">Char used to separate item name and item amount.</param>
        /// <param name="reqSep">Char used to separate each requirement.</param>
        public RequirementsParser(char amountSep = ',', char reqSep = '|')
        {
            this.reqSep = reqSep;
            this.amountSep = amountSep;
        }

        /// <summary>
        ///     Check if text is null, empty, whitespace, or equal
        ///     to the default placeholder text.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsPlaceHolderPrefabName(string text)
        {
            return string.IsNullOrWhiteSpace(text) || text == PrefabNamePlaceholder;
        }

        /// <summary>
        ///     Get a RequirementConfig with 
        ///     Item = PrefabNamePlaceholder and Amount = 0.
        /// </summary>
        /// <returns></returns>
        public static RequirementConfig GetPlaceholderReqConfig()
        {
            return new RequirementConfig(PrefabNamePlaceholder, 0);
        }

        /// <summary>
        ///     Get a RequirementConfig with Item = PrefabNamePlaceholder
        ///     and Amount = 0.
        /// </summary>
        /// <returns></returns>
        public string GetPlaceholderReqString()
        {
            return $"{PrefabNamePlaceholder}{amountSep}0";
        }

        /// <summary>
        ///     Serialize requirement configs into a string formatted as: "Req{reqSep}Req" 
        ///     where each Req value is formatted as: "ItemName{amountSep}ItemAmount{amountSep}AmountPerLevel"
        ///     if it has an amount per level value or as: "ItemName{amountSep}ItemAmount"
        ///     if it does not have an amount per level value.
        /// </summary>
        /// <param name="reqs"></param>
        /// <returns></returns>
        public string Serialize(IEnumerable<RequirementConfig> reqs)
        {
            return string.Join(
                $"{reqSep}",
                reqs.Select(
                    r => r.AmountPerLevel > 0 ? $"{r.Item}{amountSep}{r.Amount}{amountSep}{r.AmountPerLevel}" : $"{r.Item}{amountSep}{r.Amount}"
                )
            );
        }

        /// <summary>
        ///     Deserialize requirements string into a list of RequirementConfigs
        /// </summary>
        /// <param name="reqString"></param>
        /// <returns></returns>
        public List<RequirementConfig> Deserialize(string reqString)
        {
            // avoid calling Trim() on null object
            if (string.IsNullOrWhiteSpace(reqString))
            {
                return new List<RequirementConfig>() { new(PrefabNamePlaceholder, 0) };
            }

            // If not empty
            var requirements = new List<RequirementConfig>();

            foreach (string entry in reqString.Split(reqSep))
            {
                string[] values = entry.Split(amountSep);

                var reqData = new RequirementConfig()
                {
                    Item = values[0].Trim(),
                    Amount = values.Length > 1 && int.TryParse(values[1], out int amount) ? amount : 1,
                    AmountPerLevel = values.Length > 2 && int.TryParse(values[2], out int apl) ? apl : 0,
                };
                requirements.Add(reqData);
            }
            return requirements;
        }
    }
}
