using HarmonyLib;
using SuperFantasyKingdom.Buildings;

namespace SfkArchipelago.Buildings.Patches
{
    /**
     * Required kingdom level is disabled since outpost are item in the archipelago.
     */
    [HarmonyPatch(typeof(BuildingOutpost), nameof(BuildingOutpost.HasRequiredKingdomLevel))]
    public static class HasRequiredKingdomLevelPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result)
        {
            if (!ArchipelagoClient.IsConnected()) return;

            __result = true;
        }
    }
}