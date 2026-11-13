using HarmonyLib;
using SuperFantasyKingdom;

namespace SfkArchipelago.Kingdom.Patches
{
    [HarmonyPatch(typeof(RaceDataManager), nameof(RaceDataManager.IncreaseKingdomLevel))]
    public class IncreaseKingdomLevelPatch
    {
        public static bool Prefix()
        {
            if (!ArchipelagoClient.IsConnected()) return true;
            
            return false; // Avoid messing with the save.
        }
    }
}