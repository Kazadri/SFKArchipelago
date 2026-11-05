using HarmonyLib;
using SuperFantasyKingdom;

namespace SfkArchipelago.Roads.Patches
{
    [HarmonyPatch(typeof(RaceDataManager), nameof(RaceDataManager.IsRoadUnlocked))]
    public class IsRoadUnlockedPatch
    {
        [HarmonyPostfix]
        public static void Postfix(string road, ref bool __result)
        {
            if (!ArchipelagoClient.IsConnected()) return;

            if (RoadManager.IsManaged(road)) __result = RoadManager.IsGranted(road);
        }
    }
}