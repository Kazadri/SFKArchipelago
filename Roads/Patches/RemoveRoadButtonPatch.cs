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
            return !RoadUnlocks.IsManaged(road.Id);
        }
    }
}