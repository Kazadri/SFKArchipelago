using HarmonyLib;
using SuperFantasyKingdom;

namespace SfkArchipelago.Witches.Patches
{
    [HarmonyPatch(typeof(RaceDataManager), nameof(RaceDataManager.HasRaceEventFlag))]
    public class AvoidWitchSpawnPatch
    {
        public static void Postfix(RaceEvent raceEvent, ref bool __result)
        {
            var id = (int)raceEvent;
            if (id < 91 || id > 100) return; // Not a witch event

            var witchIndex = id - 90;
            __result = witchIndex <= ArchipelagoClient.WitchLevel();
        }
    }
}