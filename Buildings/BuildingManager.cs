using System.Collections.Concurrent;
using System.Collections.Generic;
using Archipelago.MultiClient.Net.Models;
using SuperFantasyKingdom.Buildings;

namespace SfkArchipelago.Buildings
{
    internal static class BuildingManager
    {
        private static readonly HashSet<BuildingType> Managed = new HashSet<BuildingType>
        {
            BuildingType.Wood,
            BuildingType.Sawmill,
            BuildingType.Well,
            BuildingType.Quarry,
            BuildingType.Farm,
            BuildingType.Tavern,
            BuildingType.Mine,
            BuildingType.Smelter,
            BuildingType.Port,
            BuildingType.Hunter,
            BuildingType.Windmill,
            BuildingType.Brewery,
            BuildingType.Siege,
            BuildingType.House,
            BuildingType.Stables,
            BuildingType.Graveyard,
            BuildingType.Portal,
            BuildingType.Bakery,
            BuildingType.Elemental,
            BuildingType.GeologistHut,
            BuildingType.Decoration,
            BuildingType.Coins,
            BuildingType.Forager,
            BuildingType.Fisher,
            BuildingType.KingdomHuman,
            BuildingType.WorldWood,
            BuildingType.WorldStone,
            BuildingType.WorldWheat,
            BuildingType.WorldOre,
            BuildingType.WorldIngot,
            BuildingType.WorldHunter,
            BuildingType.WorldBanish,
            BuildingType.WorldDifficulty,
            BuildingType.WorldFaith,
            BuildingType.WorldAppeal,
            BuildingType.WorldRelic,
            BuildingType.WorldUnit,
            BuildingType.WorldCoins,
            BuildingType.WorldBarracks,
            BuildingType.WorldArchery,
            BuildingType.WorldMages,
            BuildingType.WorldSummoners,
            BuildingType.WorldShards
        };

        private static readonly ConcurrentDictionary<int, byte> Unlocked =
            new ConcurrentDictionary<int, byte>();

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
            var type = (BuildingType)(item.ItemId - ItemIds.Building);
            if (!IsManaged(type)) return;

            Unlocked[(int)type] = 0;
            ArchipelagoClient.AddNotification($"Building unlocked: {item.ItemName}");
            Plugin.Log.LogInfo($"Building unlocked: {item.ItemName}");
        }

        internal static void SendLocation(BuildingType type)
        {
            if (!IsManaged(type)) return;

            ArchipelagoClient.SendLocation((int)type + LocationIds.Building);
        }

        internal static void Reset()
        {
            Unlocked.Clear();
        }
    }
}