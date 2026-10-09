using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using SuperFantasyKingdom.Buildings;
using Archipelago.MultiClient.Net.Helpers;


namespace SfkArchipelago
{
    internal enum ConnectionState { Disconnected, Connecting, Connected, Failed }
    internal static class ArchipelagoClient
    {
        private const long BaseId = 1_000_000;
        
        private static readonly object Sync = new object();
        private static volatile ConnectionState _state = ConnectionState.Disconnected;
        private static bool _goalSent;
        
        private static readonly ConcurrentDictionary<int, byte> UnlockedBuildings = new ConcurrentDictionary<int, byte>();
        
        private static ArchipelagoSession? _session;

        internal static readonly ConcurrentQueue<string> Notifications = new ConcurrentQueue<string>();
        
        private static readonly HashSet<int> ManagedBuildings = new HashSet<int>
        {
            1,2,3,4,5,7,8,9,10,11,12,13,14,15,16,17,18,19,27,30,32,35,36,100
        };

        internal static void StartConnect(string host, int port, string slotName, string password)
        {
            lock (Sync)
            {
                if (IsConnecting() || IsConnected())
                {
                    return;
                }
                
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

            session.Locations.CompleteLocationChecks(BaseId + value);
            Plugin.Log.LogInfo($"Check sent : Build {type} ({BaseId + value})");
        }

        internal static bool IsBuildingManaged(BuildingType type)
        {
            return ManagedBuildings.Contains((int)type);
        }
        
        internal static bool IsBuildingUnlocked(BuildingType type)
        {
            return UnlockedBuildings.ContainsKey((int)type);
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

        private static void OnItemReceived(ReceivedItemsHelper helper)
        {
            while (helper.Any())
            {
                var item = helper.DequeueItem();
                var value = item.ItemId - BaseId;
        
                if (IsBuildingManaged((BuildingType)value))
                {
                    UnlockedBuildings[(int)value] = 0;
                    Notifications.Enqueue($"Building unlocked : {item.ItemName}");
                    Plugin.Log.LogInfo($"Building unlocked : {item.ItemName}");
                }
                else
                {
                    Plugin.Log.LogInfo(
                        $"Item received (not implemented) : {item.ItemName} | id={item.ItemId} | base={BaseId} | value={value}");
                }
            }
        }
    }
}