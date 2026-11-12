using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;

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
        private static readonly object Sync = new object();
        private static volatile ConnectionState _state = ConnectionState.Disconnected;
        private static bool _goalSent;

        private static ArchipelagoSession? _session;

        internal static readonly ConcurrentQueue<string> Notifications = new ConcurrentQueue<string>();

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
                session.Items.ItemReceived += ItemDispatcher.OnItemReceived;
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

        internal static void SendLocation(long id)
        {
            var session = _session;
            if (session == null || !IsConnected())
            {
                Plugin.Log.LogWarning($"Location {id} could not be sent : not connected");
                return;
            }

            session.Locations.CompleteLocationChecks(id);
            Plugin.Log.LogInfo($"Location sent : {id})");
        }

        internal static void SendDailyCheck(int day)
        {
            var session = _session;
            if (session == null || !IsConnected())
            {
                Plugin.Log.LogWarning($"Check Daily {day} could not be sent");
                return;
            }

            session.Locations.CompleteLocationChecks(LocationIds.Daily + day);
            Plugin.Log.LogInfo($"Check sent : Day complete ({LocationIds.Daily + day})");
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
    }
}