using System.Collections.Concurrent;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using SuperFantasyKingdom;
using SuperFantasyKingdom.Buildings;
using Archipelago.MultiClient.Net.Helpers;


namespace SfkArchipelago
{
    internal static class ArchipelagoClient
    {
        private const long BaseId = 7_300_000;
        
        internal static ArchipelagoSession? Session;
        internal static bool Connected;
        private static bool _goalSent;
        internal static readonly ConcurrentDictionary<int, byte> UnlockedBuildings = new ConcurrentDictionary<int, byte>();

        internal static void Connect(string host, int port, string slotName)
        {
            Session = ArchipelagoSessionFactory.CreateSession(host, port);
            Session.Items.ItemReceived += OnItemReceived;
            

            LoginResult result = Session.TryConnectAndLogin(
                "Super Fantasy Kingdom", slotName, ItemsHandlingFlags.AllItems);

            if (result is LoginSuccessful success)
            {
                Connected = true;
                Plugin.Log.LogInfo($"Connecté à Archipelago (slot {success.Slot})");
            }
            else if (result is LoginFailure failure)
            {
                Plugin.Log.LogError("Connexion refusée : " + string.Join(", ", failure.Errors));
            }
        }
        
        internal static void SendBuildingCheck(BuildingType type)
        {
            int value = (int)type;
            if (value < 1 || value > 19) return;

            if (null == Session)
            {
                Plugin.Log.LogWarning("No Archipelago session found, the check is ignored");
                return;
            }
            
            long locationId = BaseId + value;
            Session.Locations.CompleteLocationChecks(locationId);
            Plugin.Log.LogInfo($"Check envoyé : Build {type} ({locationId})");
        }

        internal static bool IsBuildingUnlocked(BuildingType type)
        {
            return UnlockedBuildings.ContainsKey((int)type);
        }

        internal static void CompleteGoal()
        {
            if (_goalSent || !Connected || null == Session) return;
            
            Session.SetGoalAchieved();
            _goalSent = true;
            Plugin.Log.LogInfo("Goal achieved, sent to server.");
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