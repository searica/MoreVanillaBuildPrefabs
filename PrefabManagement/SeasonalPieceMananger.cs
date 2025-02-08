using Jotunn.Managers;
using Logging;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MVBP.PrefabManagement;
internal static class SeasonalPieceMananger
{
    private static bool HasInit = false;

    internal static readonly Dictionary<string, GameObject> SeasonalPiecePrefabMap = new()
    {
        {"piece_maypole", null },
        {"piece_jackoturnip", null },
        {"piece_gift1", null },
        {"piece_gift2", null },
        {"piece_gift3", null },
        {"piece_mistletoe",null },
        {"piece_xmascrown",null },
        {"piece_xmasgarland",null },
        {"piece_xmastree",null },
    };

    public static void Initialize()
    {
        if (HasInit)
        {
            return;
        }
        InitSeasonalPiecePrefabs();
        HasInit = true;
    }

    /// <summary>
    ///     Get refs to seasonal pieces that are disabled.
    /// </summary>
    private static void InitSeasonalPiecePrefabs()
    {
        List<string> nullKeys = [];
        foreach (string name in SeasonalPiecePrefabMap.Keys.ToList())
        {
            GameObject prefab = PrefabManager.Instance.GetPrefab(name);
            if (prefab && prefab.TryGetComponent(out Piece piece))
            {
                // Only add pieces that are currently disabled
                if (!piece.m_enabled)
                {
                    SeasonalPiecePrefabMap[name] = prefab;
                }
                else
                {
                    Log.LogInfo($"Seasonal Piece: {name} already enabled", Log.InfoLevel.Medium);
                    nullKeys.Add(name);
                }
            }
            else
            {
                Log.LogWarning($"Seasonal piece: {name} could not be found");
            }
        }

        foreach (string key in nullKeys)
        {
            SeasonalPiecePrefabMap.Remove(key);
        }
    }

    /// <summary>
    ///     Enables/disables seasonal pieces based on config settings.
    ///     Has no effect on seasonal pieces that are already enabled in Vanilla.
    /// </summary>
    public static void UpdateSeasonalPieces()
    {
        foreach (KeyValuePair<string, GameObject> pair in SeasonalPiecePrefabMap)
        {
            if (!pair.Value || !pair.Value.TryGetComponent(out Piece piece))
            {
                Log.LogWarning($"Invalid reference for seasonal piece {pair.Key}");
                continue;
            }
            piece.m_enabled = MorePrefabs.IsEnableSeasonalPieces;
        }
    }
}
