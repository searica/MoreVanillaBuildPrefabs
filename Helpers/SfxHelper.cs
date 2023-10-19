using Jotunn.Configs;
using MoreVanillaBuildPrefabs.Logging;
using System.Collections.Generic;
using System.Linq;

namespace MoreVanillaBuildPrefabs.Helpers
{
    /* Sound Effects
     * sfx_build_cultivator
     * sfx_build_hammer_crystal
     * sfx_build_hammer_default
     * sfx_build_hammer_metal
     * sfx_build_hammer_stone
     * sfx_build_hammer_wood
     * sfx_build_hoe
     */

    internal class SfxHelper
    {
        private static readonly Dictionary<string, EffectList.EffectData> _sfx = new();

        internal static Dictionary<string, EffectList.EffectData> SoundEffects => _sfx;

        internal static void Init()
        {
            if (_sfx.Count > 0)
            {
                _sfx.Clear();
            }

            var soundEffects = ZNetScene.instance.m_prefabs.Where(
                go => go.transform.parent == null
                && go.name.Contains("sfx_build")
            ).ToDictionary(go => go.name, go => go.gameObject);

            foreach (var key in soundEffects.Keys)
            {
                _sfx[key] = new EffectList.EffectData()
                {
                    m_prefab = soundEffects[key],
                    m_enabled = true,
                    m_variant = -1,
                };
            }
        }

        internal static void FixPlacementSfx(Piece piece)
        {
            // get effects
            var placeEffects = piece?.m_placeEffect;
            if (placeEffects == null)
            {
                piece.m_placeEffect = new EffectList();
            }
            var effects = placeEffects?.m_effectPrefabs;
            effects ??= new EffectList.EffectData[0];

            // check for sfx and enable them if needed
            foreach (var effect in effects)
            {
                if (effect.m_prefab != null
                    && effect.m_prefab.name.Contains("sfx"))
                {
                    if (!effect.m_enabled)
                    {
                        effect.m_enabled = true;
                    }
                    return;
                }
            }

            // assign sfx based on crafting station
            var craftingStation = piece?.m_craftingStation;
            if (craftingStation == null || string.IsNullOrEmpty(craftingStation?.m_name))
            {
                if (SoundEffects.ContainsKey("sfx_build_hammer_default"))
                {
                    piece.m_placeEffect.m_effectPrefabs = effects.Append(SoundEffects["sfx_build_hammer_default"]).ToArray();
                }
            }
            else if (craftingStation?.name == CraftingStations.Stonecutter)
            {
                if (SoundEffects.ContainsKey("sfx_build_hammer_stone"))
                {
                    piece.m_placeEffect.m_effectPrefabs = effects.Append(SoundEffects["sfx_build_hammer_stone"]).ToArray();
                }
            }
            else if (craftingStation?.name == CraftingStations.Workbench)
            {
                if (SoundEffects.ContainsKey("sfx_build_hammer_default"))
                {
                    piece.m_placeEffect.m_effectPrefabs = effects.Append(SoundEffects["sfx_build_hammer_default"]).ToArray();
                }
            }
            else if (craftingStation?.name == CraftingStations.Forge)
            {
                if (SoundEffects.ContainsKey("sfx_build_hammer_metal"))
                {
                    piece.m_placeEffect.m_effectPrefabs = effects.Append(SoundEffects["sfx_build_hammer_metal"]).ToArray();
                }
            }
            else if (craftingStation?.name == CraftingStations.BlackForge)
            {
                if (SoundEffects.ContainsKey("sfx_build_hammer_default"))
                {
                    piece.m_placeEffect.m_effectPrefabs = effects.Append(SoundEffects["sfx_build_hammer_default"]).ToArray();
                }
            }
        }
    }
}