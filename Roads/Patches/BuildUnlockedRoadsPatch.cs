using HarmonyLib;
using SuperFantasyKingdom;

namespace SfkArchipelago.Roads.Patches
{
    [HarmonyPatch(typeof(RoadBuilder), nameof(RoadBuilder.LoadRoads))]
    public class BuildUnlockedRoadsPatch
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            RoadUnlocks.ApplyAll();

            return false;
        }
    }
}