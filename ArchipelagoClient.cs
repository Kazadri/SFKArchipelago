using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using SfkArchipelago.Pedestals;
using SfkArchipelago.Roads;
using SuperFantasyKingdom;
using SuperFantasyKingdom.Buildings;
using Hero = SfkArchipelago.Heroes.Hero;
using ResourceManager = SfkArchipelago.Resources.ResourceManager;


namespace SfkArchipelago
{
    internal enum ConnectionState
    {
        Disconnected,
        Connecting,
        Connected,
        Failed
    }

    internal static class ArchipelagoClient
    {
        private const long BuildingBaseId = 1_000_000;
        private const long UpgradeBaseId = 1_002_000;
        private const long RoadBaseId = 1_003_000;
        private const long HeroBaseId = 1_004_000;
        private const long DailyBaseId = 1_005_000;
        private const long PedestalBaseId = 1_006_000;
        private const long WitchId = 1_007_000;

        private static readonly object Sync = new object();
        private static volatile ConnectionState _state = ConnectionState.Disconnected;
        private static bool _goalSent;

        private static readonly ConcurrentDictionary<int, byte> UnlockedBuildings =
            new ConcurrentDictionary<int, byte>();

        private static readonly ConcurrentDictionary<int, byte>
            UnlockedUpgrades = new ConcurrentDictionary<int, byte>();

        private static readonly ConcurrentDictionary<int, byte> UnlockedHeroes = new ConcurrentDictionary<int, byte>();

        private static readonly ConcurrentDictionary<int, byte> UnlockedPedestals =
            new ConcurrentDictionary<int, byte>();

        private static int _witchLevel;

        private static ArchipelagoSession? _session;

        internal static readonly ConcurrentQueue<string> Notifications = new ConcurrentQueue<string>();

        private static readonly HashSet<BuildingType> ManagedBuildings = new HashSet<BuildingType>
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

        private static readonly HashSet<BuildingType> ManagedUpgrades = new HashSet<BuildingType>
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


        internal static void StartConnect(string host, int port, string slotName, string password)
        {
            lock (Sync)
            {
                if (IsConnecting() || IsConnected()) return;

                _state = ConnectionState.Connecting;
            }

            Task.Run(() => ConnectWorker(host, port, slotName, password));
        }

        private static void ConnectWorker(string host, int port, string slotName, string password)
        {
            try
            {
                var session = ArchipelagoSessionFactory.CreateSession(host, port);
                session.Items.ItemReceived += OnItemReceived;
                session.Socket.SocketClosed += OnSockedClosed;

                var result = session.TryConnectAndLogin(
                    "Super Fantasy Kingdom", slotName, ItemsHandlingFlags.AllItems,
                    password: string.IsNullOrEmpty(password) ? null : password);

                switch (result)
                {
                    case LoginSuccessful success:
                        _session = session;
                        _state = ConnectionState.Connected;
                        Plugin.Log.LogInfo($"Connecté à Archipelago (slot {success.Slot})");
                        break;
                    case LoginFailure failure:
                        Fail("Connexion refusée : " + string.Join(", ", failure.Errors));
                        break;
                }
            }
            catch (Exception e)
            {
                Fail("Erreur de connexion : " + e.Message);
                Plugin.Log.LogError(e.ToString());
            }
        }

        private static void Fail(string message)
        {
            _state = ConnectionState.Failed;
            Plugin.Log.LogError(message);
        }

        private static void OnSockedClosed(string reason)
        {
            if (!IsConnected()) return;

            _state = ConnectionState.Disconnected;
            _session = null;
            Plugin.Log.LogWarning($"Lost connection : {reason}");
        }

        internal static void SendBuildingCheck(BuildingType type)
        {
            var value = (int)type;
            if (!IsBuildingManaged(type)) return;

            var session = _session;
            if (session == null || !IsConnected())
            {
                Plugin.Log.LogWarning($"Check Build {type} could not be sent : not connected");
                return;
            }

            session.Locations.CompleteLocationChecks(BuildingBaseId + value);
            Plugin.Log.LogInfo($"Check sent : Build {type} ({BuildingBaseId + value})");
        }

        internal static void SendDailyCheck(int day)
        {
            var session = _session;
            if (session == null || !IsConnected())
            {
                Plugin.Log.LogWarning($"Check Daily {day} could not be sent");
                return;
            }

            session.Locations.CompleteLocationChecks(DailyBaseId + day);
            Plugin.Log.LogInfo($"Check sent : Day complete ({DailyBaseId + day})");
        }

        internal static bool IsBuildingManaged(BuildingType type)
        {
            return ManagedBuildings.Contains(type);
        }

        internal static bool IsBuildingUnlocked(BuildingType type)
        {
            return UnlockedBuildings.ContainsKey((int)type);
        }

        internal static bool IsUpgradeManaged(BuildingType type)
        {
            return ManagedUpgrades.Contains(type);
        }

        internal static bool IsUpgradeUnlocked(BuildingType type)
        {
            return false; //TODO
        }

        internal static bool IsHeroManaged(Hero hero)
        {
            return true; // All heroes are managed
        }

        internal static bool IsHeroUnlocked(Hero hero)
        {
            return UnlockedHeroes.ContainsKey((int)hero);
        }

        internal static bool IsPedestalUnlocked(Pedestal pedestal)
        {
            return UnlockedPedestals.ContainsKey(pedestal.Offset);
        }

        internal static int WitchLevel()
        {
            return _witchLevel;
        }

        private static void IncrementWitchLevel()
        {
            ++_witchLevel;
        }

        internal static void CompleteGoal()
        {
            var session = _session;
            if (_goalSent || null == session || !IsConnected()) return;

            _goalSent = true;
            session.SetGoalAchieved();
            Plugin.Log.LogInfo("Goad achieved, sent to server");
        }

        internal static bool IsConnected()
        {
            return ConnectionState.Connected == _state;
        }

        internal static bool IsConnecting()
        {
            return ConnectionState.Connecting == _state;
        }

        public static void AddNotification(string message)
        {
            Notifications.Enqueue(message);
        }

        private static void OnItemReceived(ReceivedItemsHelper helper)
        {
            while (helper.Any())
            {
                var item = helper.DequeueItem();

                switch (ResolveItemKind(item.ItemId))
                {
                    case ItemKind.Building:
                        HandleBuildingItem(item);
                        break;
                    case ItemKind.Upgrade:
                        HandleUpgradeItem(item);
                        break;
                    case ItemKind.Road:
                        HandleRoadItem(item);
                        break;
                    case ItemKind.Hero:
                        HandleHeroItem(item);
                        break;
                    case ItemKind.Pedestal:
                        HandlePedestalItem(item);
                        break;
                    case ItemKind.Witch:
                        HandleWitchItem();
                        break;
                    case ItemKind.Filler:
                        HandleFillerItem(item);
                        break;
                    default:
                        Plugin.Log.LogInfo(
                            $"Item received (not implemented) : {item.ItemName} | id={item.ItemId}");
                        break;
                }

                ;
            }
        }

        private static void HandleFillerItem(ItemInfo item)
        {
            var type = ResourceManager.NameToType(item.ItemName);
            if (null == type) return;

            ResourceManager.AddResource((ResourceType)type);
        }

        private static void HandleBuildingItem(ItemInfo item)
        {
            var value = item.ItemId - BuildingBaseId;

            if (!IsBuildingManaged((BuildingType)value)) return;

            UnlockedBuildings[(int)value] = 0;
            AddNotification($"Building unlocked : {item.ItemName}");
            Plugin.Log.LogInfo($"Building unlocked : {item.ItemName}");
        }

        private static void HandleUpgradeItem(ItemInfo item)
        {
            var value = item.ItemId - UpgradeBaseId;

            if (!IsUpgradeManaged((BuildingType)value)) return;

            UnlockedUpgrades[(int)value] = 0;
            AddNotification($"Upgrade unlocked : {item.ItemName}");
            Plugin.Log.LogInfo($"Upgrade unlocked : {item.ItemName}");
        }

        private static void HandleRoadItem(ItemInfo item)
        {
            if (!Enum.IsDefined(typeof(RoadGroup), (int)item.ItemId))
            {
                Plugin.Log.LogWarning($"Unknown road item id {item.ItemId} ({item.ItemName})");
                return;
            }

            var group = (RoadGroup)(int)item.ItemId;

            RoadUnlocks.IncrementLevel(group);

            AddNotification($"New road unlocked: {group.ToString()}");
            Plugin.Log.LogInfo($"New road unlocked: {group.ToString()}");
        }

        private static void HandleHeroItem(ItemInfo item)
        {
            var value = item.ItemId - HeroBaseId;

            if (!Enum.IsDefined(typeof(Hero), (int)value))
            {
                Plugin.Log.LogWarning($"Unknown hero item id {item.ItemId} ({item.ItemName})");
                return;
            }

            UnlockedHeroes[(int)value] = 0;
            AddNotification($"Unlocked hero: {item.ItemName}");
            Plugin.Log.LogInfo($"Unlocked hero: {item.ItemName}");
        }

        private static void HandlePedestalItem(ItemInfo item)
        {
            var value = item.ItemId - PedestalBaseId;

            UnlockedPedestals[(int)value] = 0;
            AddNotification($"Unlocked pedestal: {item.ItemName}");
            Plugin.Log.LogInfo($"Unlocked pedestal: {item.ItemName}");
        }

        private static void HandleWitchItem()
        {
            IncrementWitchLevel();

            AddNotification($"Witch level unlocked: {WitchLevel().ToString()}");
            Plugin.Log.LogInfo($"Witch level unlocked: {WitchLevel().ToString()}");
        }

        private static ItemKind ResolveItemKind(long itemId)
        {
            if (itemId >= 3_000_000) return ItemKind.Unknow;
            if (itemId >= 2_000_000) return ItemKind.Filler;
            if (itemId == WitchId) return ItemKind.Witch;
            if (itemId >= PedestalBaseId) return ItemKind.Pedestal;
            if (itemId >= HeroBaseId) return ItemKind.Hero;
            if (itemId >= RoadBaseId) return ItemKind.Road;
            if (itemId >= UpgradeBaseId) return ItemKind.Upgrade;
            if (itemId >= BuildingBaseId) return ItemKind.Building;

            return ItemKind.Unknow;
        }

        private enum ItemKind
        {
            Building,
            Upgrade,
            Road,
            Hero,
            Pedestal,
            Witch,
            Filler,
            Unknow
        }
    }
}