using System.Collections.Concurrent;
using System.Collections.Generic;
using Archipelago.MultiClient.Net.Models;
using SuperFantasyKingdom.Buildings;

namespace SfkArchipelago.Buildings
{
    internal static class UpgradeManager
    {
        private static readonly HashSet<BuildingType> Managed = new HashSet<BuildingType>
        {
            BuildingType.Wood,
            BuildingType.Well,
            BuildingType.Quarry,
            BuildingType.Farm,
            BuildingType.Tavern,
            BuildingType.Mine,
            BuildingType.Port,
            BuildingType.Hunter,
            BuildingType.House,
            BuildingType.GeologistHut,
            BuildingType.Decoration,
            BuildingType.Forager,
            BuildingType.Fisher
        };

        private static readonly ConcurrentDictionary<int, byte>
            Unlocked = new ConcurrentDictionary<int, byte>();

        internal static bool IsManaged(BuildingType type)
        {
            return Managed.Contains(type);
        }

        internal static bool IsUnlocked(BuildingType type)
        {
            return Unlocked.ContainsKey((int)type);
        }

        internal static void Handle(ItemInfo item)
        {
            var type = (BuildingType)(item.ItemId - ItemIds.Upgrade);
            if (!IsManaged(type)) return;

            Unlocked[(int)type] = 0;
            ArchipelagoClient.AddNotification($"Upgrade unlocked: {item.ItemName}");
            Plugin.Log.LogInfo($"Upgrade unlocked: {item.ItemName}");
        }

        internal static void Reset()
        {
            Unlocked.Clear();
        }
    }
}