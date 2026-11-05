using System;
using System.Collections.Generic;
using System.Threading;
using Archipelago.MultiClient.Net.Models;
using SuperFantasyKingdom;

namespace SfkArchipelago.Buildings
{
    internal static class CastleManager
    {
        private static readonly int[] Levels = new int[5];

        private static readonly Dictionary<CityEvent, (CastleGroup Group, int Tier)> Requirements =
            new Dictionary<CityEvent, (CastleGroup Group, int Tier)>
            {
                { CityEvent.UpgradeBase1, (CastleGroup.Castle, 1) },
                { CityEvent.UpgradeBase2, (CastleGroup.Castle, 2) },
                { CityEvent.UpgradeBase3, (CastleGroup.Castle, 3) },
                { CityEvent.WallsWood, (CastleGroup.Walls, 1) },
                { CityEvent.WallsStone, (CastleGroup.Walls, 2) },
                { CityEvent.WallsGold, (CastleGroup.Walls, 3) },
                { CityEvent.GateWood, (CastleGroup.Gate, 1) },
                { CityEvent.GateStone, (CastleGroup.Gate, 2) },
                { CityEvent.GateGold, (CastleGroup.Gate, 3) },
                { CityEvent.TowerWoodLeft, (CastleGroup.TowerLeft, 1) },
                { CityEvent.TowerStoneLeft, (CastleGroup.TowerLeft, 2) },
                { CityEvent.TowerGoldLeft, (CastleGroup.TowerLeft, 3) },
                { CityEvent.TowerWoodRight, (CastleGroup.TowerRight, 1) },
                { CityEvent.TowerStoneRight, (CastleGroup.TowerRight, 2) },
                { CityEvent.TowerGoldRight, (CastleGroup.TowerRight, 3) }
            };

        internal static int Level(CastleGroup g)
        {
            return Volatile.Read(ref Levels[(int)g]);
        }

        internal static bool IsManaged(CityEvent e)
        {
            return Requirements.ContainsKey(e);
        }

        internal static bool IsUnlocked(CityEvent e)
        {
            return Requirements.TryGetValue(e, out var req) && Level(req.Group) >= req.Tier;
        }

        internal static void Handle(ItemInfo item)
        {
            var group = (CastleGroup)(int)(item.ItemId - ItemIds.Castle);
            if (!Enum.IsDefined(typeof(CastleGroup), group))
            {
                Plugin.Log.LogWarning($"Unknown castle item {item.ItemId} ({item.ItemName})");
                return;
            }

            var level = Interlocked.Increment(ref Levels[(int)group]);

            ArchipelagoClient.AddNotification($"Progressive {group}: level {level}");
            Plugin.Log.LogInfo($"Progressive {group}: level {level}");
        }


        internal static void Reset()
        {
            for (var i = 0; i < Levels.Length; i++) Volatile.Write(ref Levels[i], 0);
        }
    }
}