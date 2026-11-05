using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Archipelago.MultiClient.Net.Models;
using HarmonyLib;
using SfkArchipelago.Races;
using SuperFantasyKingdom;
using SuperFantasyKingdom.Clickable;
using UnityEngine;

namespace SfkArchipelago.Pedestals
{
    internal static class PedestalManager
    {
        private static readonly ConcurrentDictionary<int, byte> Unlocked =
            new ConcurrentDictionary<int, byte>();

        public static bool InMagic;

        private static readonly AccessTools.FieldRef<BuildingSpotWorld, ResourceType> ResourceRef =
            AccessTools.FieldRefAccess<BuildingSpotWorld, ResourceType>("resource");

        private static readonly List<Pedestal> All = new List<Pedestal>
        {
            new Pedestal("Berry1", 0, Race.Human),
            new Pedestal("Berry2", 1, Race.Human),
            new Pedestal("Berry3", 2, Race.Human),
            new Pedestal("Berry4", 3, Race.Human),
            new Pedestal("Gold1", 4, Race.Human),
            new Pedestal("Gold2", 5, Race.Human),
            new Pedestal("Gold3", 6, Race.Human),
            new Pedestal("Gold4", 7, Race.Human),
            new Pedestal("Gold5", 8, Race.Human),
            new Pedestal("Stone1", 9, Race.Human),
            new Pedestal("Stone2", 10, Race.Human),
            new Pedestal("Stone3", 11, Race.Human),
            new Pedestal("Stone4", 12, Race.Human),
            new Pedestal("Stone5", 13, Race.Human),
            new Pedestal("Stone6", 14, Race.Human),
            new Pedestal("Stone7", 15, Race.Human),
            new Pedestal("Wheat1", 16, Race.Human),
            new Pedestal("Wheat2", 17, Race.Human),
            new Pedestal("Wheat3", 18, Race.Human),
            new Pedestal("Wheat4", 19, Race.Human),
            new Pedestal("Wheat5", 20, Race.Human),
            new Pedestal("Wheat6", 21, Race.Human),
            new Pedestal("Wood1", 22, Race.Human),
            new Pedestal("Wood2", 23, Race.Human),
            new Pedestal("Wood3", 24, Race.Human),
            new Pedestal("Wood4", 25, Race.Human),
            new Pedestal("Wood5", 26, Race.Human),
            new Pedestal("Wood6", 27, Race.Human),
            new Pedestal("Wood7", 28, Race.Human)
        };

        private static readonly Dictionary<(Race, string), Pedestal> ByKey =
            All.ToDictionary(p => (p.Race, p.Name));

        public static bool IsUnlocked(Pedestal p)
        {
            return Unlocked.ContainsKey(p.Offset);
        }

        internal static void Handle(ItemInfo item)
        {
            var offset = (int)(item.ItemId - ItemIds.Pedestal);

            Unlocked[offset] = 0;

            ArchipelagoClient.AddNotification($"Unlocked pedestal: {item.ItemName}");
            Plugin.Log.LogInfo($"Unlocked pedestal: {item.ItemName}");
        }

        internal static void Reset()
        {
            Unlocked.Clear();
        }

        public static bool TryGet(Race race, string name, out Pedestal p)
        {
            return ByKey.TryGetValue((race, name), out p);
        }

        public static void SpawnAllUnlocked()
        {
            foreach (var spot in Object.FindObjectsOfType<BuildingSpotWorld>())
                if (TryGet(RaceUtils.CurrentRace(), spot.name, out var p) && IsUnlocked(p))
                    WorldManager.Instance.SpawnRebuild(spot.transform.position, ResourceRef(spot), true);
        }
    }
}