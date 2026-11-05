using HarmonyLib;
using SuperFantasyKingdom;

namespace SfkArchipelago.Roads.Patches
{
    [HarmonyPatch(typeof(RoadBuilder), nameof(RoadBuilder.BuildWholeRoad))]
    public class BuildWhoreRoadPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(string road)
        {
            if (!ArchipelagoClient.IsConnected()) return true;

            return !RoadManager.IsManaged(road) || RoadManager.IsGranted(road);
        }
    }
}