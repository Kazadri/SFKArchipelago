using HarmonyLib;
using SuperFantasyKingdom;
using SuperFantasyKingdom.Buildings;

namespace SfkArchipelago
{
    [HarmonyPatch(typeof(CityManager), nameof(CityManager.CanBuild))]
    public static class CanBuildPatch
    {
        [HarmonyPostfix]
        public static void Postfix(BuildingType type, ref bool __result)
        {
            if (!ArchipelagoClient.IsConnected()) return;
            
            if (!ArchipelagoClient.IsBuildingManaged(type)) return;

            if (!ArchipelagoClient.IsBuildingUnlocked(type))
            {
                __result = false;
            }
        }
    }
}