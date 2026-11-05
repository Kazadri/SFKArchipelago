using System;
using System.Collections.Concurrent;
using Archipelago.MultiClient.Net.Models;

namespace SfkArchipelago.Heroes
{
    internal static class HeroManager
    {
        private static readonly ConcurrentDictionary<int, byte> Unlocked = new ConcurrentDictionary<int, byte>();

        internal static bool IsManaged(Hero hero)
        {
            return true;
            // All heroes are managed
        }

        internal static bool IsUnlocked(Hero hero)
        {
            return Unlocked.ContainsKey((int)hero);
        }

        internal static void Handle(ItemInfo item)
        {
            var value = item.ItemId - ItemIds.Hero;

            if (!Enum.IsDefined(typeof(Hero), (int)value))
            {
                Plugin.Log.LogWarning($"Unknown hero item id {item.ItemId} ({item.ItemName})");
                return;
            }

            Unlocked[(int)value] = 0;
            ArchipelagoClient.AddNotification($"Unlocked hero: {item.ItemName}");
            Plugin.Log.LogInfo($"Unlocked hero: {item.ItemName}");
        }

        internal static void Reset()
        {
            Unlocked.Clear();
        }
    }
}