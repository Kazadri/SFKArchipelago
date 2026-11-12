using System;
using System.Collections.Concurrent;
using Archipelago.MultiClient.Net.Models;

namespace SfkArchipelago.Units
{
    public class UnitManager
    {
        private static readonly ConcurrentDictionary<int, byte> Unlocked = new ConcurrentDictionary<int, byte>();

        internal static bool IsManger(string unit)
        {
            return true; // All unit are managed
        }

        internal static bool IsUnlocked(Unit unit)
        {
            return Unlocked.ContainsKey((int)unit);
        }

        internal static void Handle(ItemInfo item)
        {
            var value = item.ItemId - ItemIds.Unit;

            if (!Enum.IsDefined(typeof(Unit), (int)value))
            {
                Plugin.Log.LogWarning($"Unknown unit item id {item.ItemId} ({item.ItemName})");
                return;
            }

            Unlocked[(int)value] = 0;
            ArchipelagoClient.AddNotification($"Unlocked unit: {item.ItemName}");
            Plugin.Log.LogInfo($"Unlocked unit: {item.ItemName}");
        }

        internal static void Reset()
        {
            Unlocked.Clear();
        }
    }
}