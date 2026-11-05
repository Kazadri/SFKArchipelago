using HarmonyLib;
using SuperFantasyKingdom;

namespace SfkArchipelago.Roads.Patches
{
    [HarmonyPatch(typeof(RoadBuilder), "AddRoadButton")]
    public class RemoveRoadButtonPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Road road)
        {
            if (!ArchipelagoClient.IsConnected()) return true;

            return !RoadManager.IsManaged(road.Id);
        }
    }
}