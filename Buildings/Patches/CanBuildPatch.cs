using HarmonyLib;
using SuperFantasyKingdom;
using SuperFantasyKingdom.Buildings;

namespace SfkArchipelago.Buildings.Patches
{
    [HarmonyPatch(typeof(CityManager), nameof(CityManager.CanBuild))]
    public static class CanBuildPatch
    {
        [HarmonyPostfix]
        public static void Postfix(BuildingType type, ref bool __result)
        {
            if (!ArchipelagoClient.IsConnected()) return;

            if (!BuildingManager.IsManaged(type)) return;

            if (!BuildingManager.IsUnlocked(type)) __result = false;
        }
    }
}