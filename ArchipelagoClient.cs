using System;
using System.Collections.Concurrent;
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
        
        internal static readonly ConcurrentDictionary<int, byte> UnlockedBuildings = new ConcurrentDictionary<int, byte>();
        
        internal static ArchipelagoSession? Session;
        internal static volatile string LastError = "";

        internal static ConnectionState State => _state;
        
        internal static bool EnforceUnlocks =>
            _state == ConnectionState.Connecting || _state == ConnectionState.Connected;
        
        internal static readonly ConcurrentQueue<string> Notifications = new ConcurrentQueue<string>();

        internal static void StartConnect(string host, int port, string slotName, string password)
        {
            lock (Sync)
            {
                if (ConnectionState.Connecting == _state || ConnectionState.Connected == _state)
                {
                    return;
                }
                
                _state = ConnectionState.Connecting;
                LastError = "";
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
                
                LoginResult result = session.TryConnectAndLogin(
                    "Super Fantasy Kingdom", slotName, ItemsHandlingFlags.AllItems,
                    password: string.IsNullOrEmpty(password) ? null : password);
                
                if (result is LoginSuccessful success)
                {
                    Session = session;
                    _state = ConnectionState.Connected;
                    Plugin.Log.LogInfo($"Connecté à Archipelago (slot {success.Slot})");
                }
                else if (result is LoginFailure failure)
                {
                    Fail("Connexion refusée : " + string.Join(", ", failure.Errors));
                }
            }
            catch (Exception e)
            {
                // Sur un Task.Run, une exception non attrapée disparaît en silence.
                Fail("Erreur de connexion : " + e.Message);
                Plugin.Log.LogError(e.ToString());
            }
        }

        private static void Fail(string message)
        {
            LastError = message;
            _state = ConnectionState.Failed;
            Plugin.Log.LogError(message);
        }

        private static void OnSockedClosed(string reason)
        {
            if (ConnectionState.Connected != _state) return;

            _state = ConnectionState.Disconnected;
            Session = null;
            Plugin.Log.LogWarning($"Connexion perdue : {reason}");
        }

        internal static void SendBuildingCheck(BuildingType type)
        {
            int value = (int)type;
            if (value < 1 || value > 19) return;

            var session = Session;
            if (session == null || _state != ConnectionState.Connected)
            {
                Plugin.Log.LogWarning($"Check Build {type} non envoyé : pas connecté");
                return;
            }

            session.Locations.CompleteLocationChecks(BaseId + value);
            Plugin.Log.LogInfo($"Check envoyé : Build {type} ({BaseId + value})");
        }
        
        internal static bool IsBuildingUnlocked(BuildingType type)
        {
            return UnlockedBuildings.ContainsKey((int)type);
        }

        internal static void CompleteGoal()
        {
            var session = Session;
            if (_goalSent || session == null || _state != ConnectionState.Connected) return;

            _goalSent = true;
            session.SetGoalAchieved();
            Plugin.Log.LogInfo("Objectif atteint, envoyé au serveur");
        }

        private static void OnItemReceived(ReceivedItemsHelper helper)
        {
            while (helper.Any())
            {
                var item = helper.DequeueItem();
                long value = item.ItemId - BaseId;
        
                if (value >= 1 && value <= 19)
                {
                    UnlockedBuildings[(int)value] = 0;
                    Notifications.Enqueue($"Bâtiment débloqué : {item.ItemName}");
                    Plugin.Log.LogInfo($"Bâtiment débloqué : {item.ItemName}");
                }
                else
                {
                    Plugin.Log.LogInfo(
                        $"Item reçu (ignoré) : {item.ItemName} | id={item.ItemId} | base={BaseId} | value={value}");
                }
            }
        }
    }
}