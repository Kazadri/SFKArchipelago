using System;
using System.Collections.Generic;
using System.Threading;
using Archipelago.MultiClient.Net.Models;
using SuperFantasyKingdom;

namespace SfkArchipelago.Roads
{
    internal static class RoadManager
    {
        private static readonly Dictionary<RoadGroup, RoadGroupState> Groups = new Dictionary<RoadGroup, RoadGroupState>
        {
            { RoadGroup.Overland, new RoadGroupState() },
            { RoadGroup.City, new RoadGroupState() }
        };

        private static Dictionary<string, (RoadGroup Group, int Index)> _lookup =
            new Dictionary<string, (RoadGroup, int)>();

        public static bool IsManaged(string id)
        {
            return _lookup.ContainsKey(id);
        }

        public static bool IsGranted(string id)
        {
            return _lookup.TryGetValue(id, out var entry)
                   && entry.Index < Volatile.Read(ref Groups[entry.Group].Level);
        }

        public static void ApplyAll()
        {
            if (RoadBuilder.Instance == null) return;

            foreach (var kv in Groups)
            {
                var state = kv.Value;
                var level = Volatile.Read(ref state.Level);

                for (var i = 0; i < level && i < state.Order.Length; i++)
                {
                    var id = state.Order[i];
                    try
                    {
                        RoadBuilder.Instance.BuildWholeRoad(id);
                    }
                    catch (Exception e)
                    {
                        Plugin.Log.LogError($"Road {id}: {e}");
                    }
                }
            }

            PathManager.Instance?.RedoGrid();
        }

        public static void IncrementLevel(RoadGroup group)
        {
            Interlocked.Increment(ref Groups[group].Level);
            ApplyAll();
        }

        public static void SetOrder(RoadGroup group, string[] order)
        {
            Groups[group].Order = order;

            // Rebuild the lookup from all groups, then swap the reference
            var lookup = new Dictionary<string, (RoadGroup, int)>();
            foreach (var kv in Groups)
                for (var i = 0; i < kv.Value.Order.Length; i++)
                    lookup[kv.Value.Order[i]] = (kv.Key, i);
            _lookup = lookup;
        }

        internal static void Handle(ItemInfo item)
        {
            var group = (RoadGroup)(int)item.ItemId;
            if (!Groups.ContainsKey(group))
            {
                Plugin.Log.LogWarning($"Unknown road item id {item.ItemId} ({item.ItemName})");
                return;
            }

            IncrementLevel(group);
            ArchipelagoClient.AddNotification($"New road unlocked: {group}");
            Plugin.Log.LogInfo($"New road unlocked: {group}");
        }

        internal static void Reset()
        {
            foreach (var state in Groups.Values)
                Volatile.Write(ref state.Level, 0);
        }

        private class RoadGroupState
        {
            public int Level;
            public string[] Order = Array.Empty<string>();
        }
    }
}