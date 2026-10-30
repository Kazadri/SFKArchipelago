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
            if (RoadUnlocks.IsManaged(road)) __result = RoadUnlocks.IsGranted(road);
        }
    }
}