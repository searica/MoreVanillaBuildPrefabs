using Jotunn.Configs;
using Jotunn.Managers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MVBP.Helpers
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
        private static readonly Dictionary<string, EffectList.EffectData> PlacementSfx = new()
        {
            {"sfx_build_hammer_metal", null },
            {"sfx_build_hammer_stone", null},
            {"sfx_build_hammer_default", null},
        };

        private static readonly Dictionary<string, EffectList.EffectData> RemovalSfx = new()
        {
            { "sfx_rock_destroyed", null },
            { "sfx_wood_destroyed", null },
            { "sfx_treasurechest_destroyed", null },
            { "sfx_ship_destroyed", null }
        };

        /// <summary>
        ///     Initializes SfxHelper and caches SFX prefabs.
        /// </summary>
        internal static void Init()
        {
            InitSfx(PlacementSfx);
            InitSfx(RemovalSfx);
        }

        /// <summary>
        ///     Finds prefabs and initializes dictionary of SFX
        /// </summary>
        /// <param name="sfxDict"></param>
        private static void InitSfx(Dictionary<string, EffectList.EffectData> sfxDict)
        {
            var sfxNames = sfxDict.Keys.ToList();
            var nullKeys = new List<string>();
            foreach (var name in sfxNames)
            {
                var prefab = PrefabManager.Instance.GetPrefab(name);
                if (prefab != null)
                {
                    sfxDict[name] = CreateEffectData(prefab);
                }
                else
                {
                    nullKeys.Add(name);
                }
            }
            foreach (var key in nullKeys)
            {
                Log.LogWarning($"Sfx: {key} could not be found.");
                sfxDict.Remove(key);
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
            if (craftingStation == null
                || string.IsNullOrEmpty(craftingStation?.m_name)
                || craftingStation?.name == CraftingStations.Workbench
                || craftingStation?.name == CraftingStations.BlackForge)
            {
                if (PlacementSfx.TryGetValue("sfx_build_hammer_default", out EffectList.EffectData val))
                {
                    effectsList.Add(val);
                }
            }
            else if (craftingStation?.name == CraftingStations.Stonecutter)
            {
                if (PlacementSfx.TryGetValue("sfx_build_hammer_stone", out EffectList.EffectData val))
                {
                    effectsList.Add(val);
                }
            }
            else if (craftingStation?.name == CraftingStations.Forge)
            {
                if (PlacementSfx.TryGetValue("sfx_build_hammer_metal", out EffectList.EffectData val))
                {
                    effectsList.Add(val);
                }
            }

            piece.m_placeEffect.m_effectPrefabs = effectsList.ToArray();
        }

        internal static void CreateRemovalSfx(Piece piece)
        {
            var effects = FixRemovalSfx(piece);
            effects.Create(
                piece.transform.position,
                piece.transform.rotation,
                piece.gameObject.transform
            );
        }

        /// <summary>
        ///     Copies placeEffect list and adds sfx based on
        ///     required crafting station if needed.
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        internal static EffectList FixRemovalSfx(Piece piece)
        {
            return FixRemovalSfx(piece?.m_placeEffect, piece?.m_craftingStation);
        }

        /// <summary>
        ///     Copies destroyedEffect list and adds sfx based on
        ///     required crafting station if needed.
        /// </summary>
        /// <param name="wearNTear"></param>
        /// <returns></returns>
        internal static EffectList FixRemovalSfx(WearNTear wearNTear)
        {
            var craftingStation = wearNTear?.gameObject?.GetComponent<Piece>()?.m_craftingStation;
            return FixRemovalSfx(wearNTear?.m_destroyedEffect, craftingStation);
        }

        /// <summary>
        ///     Copies effectList and adds sfx based on
        ///     required crafting station if needed.
        /// </summary>
        internal static EffectList FixRemovalSfx(
            EffectList effectList,
            CraftingStation craftingStation
        )
        {
            var effects = (effectList?.m_effectPrefabs) ?? (new EffectList.EffectData[0]);
            var effectsList = effects.ToList();
            if (craftingStation != null && craftingStation.name == CraftingStations.Stonecutter)
            {
                if (RemovalSfx.TryGetValue("sfx_rock_destroyed", out EffectList.EffectData val))
                {
                    effectsList.Add(val);
                }
            }
            else if (RemovalSfx.TryGetValue("sfx_wood_destroyed", out EffectList.EffectData val))
            {
                effectsList.Add(val);
            }

            return new EffectList() { m_effectPrefabs = effectsList.ToArray() };
        }

        internal static bool HasSfx(EffectList effectList)
        {
            var effects = effectList?.m_effectPrefabs;
            if (effects != null && effects.Length != 0)
            {
                foreach (var effect in effects)
                {
                    if (effect != null && effect.m_prefab.name.StartsWith("sfx_"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}