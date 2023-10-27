using Jotunn.Configs;
using MoreVanillaBuildPrefabs.Configs;
using MoreVanillaBuildPrefabs.Logging;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MoreVanillaBuildPrefabs.Helpers
{
    /* Placement Sound Effects
     * sfx_build_cultivator
     * sfx_build_hammer_crystal
     * sfx_build_hammer_default
     * sfx_build_hammer_metal
     * sfx_build_hammer_stone
     * sfx_build_hammer_wood
     * sfx_build_hoe
     */

    /* Removal Sound Effects
     * sfx_barnacle_destroyed
     * sfx_beehive_destroyed
     * sfx_bonepile_destroyed
     * sfx_coins_destroyed
     * sfx_coins_pile_destroyed
     * sfx_draugrpile_destroyed
     * sfx_gdking_rock_destroyed
     * sfx_greydwarfnest_destroyed
     * sfx_GuckSackDestroyed
     * sfx_ice_destroyed
     * sfx_MudDestroyed
     * sfx_rock_destroyed
     * sfx_ship_destroyed
     * sfx_treasurechest_destroyed
     * sfx_troll_rock_destroyed
     * sfx_wood_destroyed
     */

    // disable IceBlocker

    internal class SfxHelper
    {
        private static readonly Dictionary<string, EffectList.EffectData> PlacementSfx = new();

        private static readonly Dictionary<string, EffectList.EffectData> RemovalSfx = new()
        {
            { "sfx_rock_destroyed", null },
            { "sfx_wood_destroyed", null },
            { "sfx_treasurechest_destroyed", null },
            { "sfx_ship_destroyed", null }
        };

        internal static void Init()
        {
            if (PlacementSfx.Count > 0) { PlacementSfx.Clear(); }

            foreach (var prefab in ZNetScene.instance.m_prefabs)
            {
                if (prefab.transform.parent != null) { continue; }
                if (prefab.name.Contains("sfx_build"))
                {
                    PlacementSfx.Add(prefab.name, CreateEffectData(prefab));
                }
                else if (RemovalSfx.ContainsKey(prefab.name))
                {
                    RemovalSfx[prefab.name] = CreateEffectData(prefab);
                }
            }
        }

        private static EffectList.EffectData CreateEffectData(GameObject prefab)
        {
            return new EffectList.EffectData()
            {
                m_prefab = prefab,
                m_enabled = true,
                m_variant = -1,
            };
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
            var effectsList = effects.ToList();
            if (craftingStation == null || string.IsNullOrEmpty(craftingStation?.m_name))
            {
                if (PlacementSfx.ContainsKey("sfx_build_hammer_default"))
                {
                    effectsList.Add(PlacementSfx["sfx_build_hammer_default"]);
                }
            }
            else if (craftingStation?.name == CraftingStations.Stonecutter)
            {
                if (PlacementSfx.ContainsKey("sfx_build_hammer_stone"))
                {
                    effectsList.Add(PlacementSfx["sfx_build_hammer_stone"]);
                }
            }
            else if (craftingStation?.name == CraftingStations.Workbench)
            {
                if (PlacementSfx.ContainsKey("sfx_build_hammer_default"))
                {
                    effectsList.Add(PlacementSfx["sfx_build_hammer_default"]);
                }
            }
            else if (craftingStation?.name == CraftingStations.Forge)
            {
                if (PlacementSfx.ContainsKey("sfx_build_hammer_metal"))
                {
                    effectsList.Add(PlacementSfx["sfx_build_hammer_metal"]);
                }
            }
            else if (craftingStation?.name == CraftingStations.BlackForge)
            {
                if (PlacementSfx.ContainsKey("sfx_build_hammer_default"))
                {
                    effectsList.Add(PlacementSfx["sfx_build_hammer_default"]);
                }
            }
            piece.m_placeEffect.m_effectPrefabs = effectsList.ToArray();
        }

        internal static EffectList FixRemovalSfx(Piece piece)
        {
            var effects = (piece.m_placeEffect?.m_effectPrefabs) ?? (new EffectList.EffectData[0]);
            var effectsList = effects.ToList();
            var craftingStation = piece?.m_craftingStation;
            if (craftingStation != null && craftingStation?.name == CraftingStations.Stonecutter)
            {
                effectsList.Add(RemovalSfx["sfx_rock_destroyed"]);
            }
            else
            {
                effectsList.Add(RemovalSfx["sfx_wood_destroyed"]);
            }

            return new EffectList()
            {
                m_effectPrefabs = effectsList.ToArray()
            };
        }
    }
}