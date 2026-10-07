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
            if (!ArchipelagoClient.Connected) return;
            
            int value = (int)type;
            if (value < 1 || value > 19) return;

            if (!ArchipelagoClient.IsBuildingUnlocked(type))
            {
                __result = false;
            }
        }
    }
}