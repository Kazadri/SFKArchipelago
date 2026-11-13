using HarmonyLib;
using SuperFantasyKingdom;

namespace SfkArchipelago.Kingdom.Patches
{
    [HarmonyPatch(typeof(RaceDataManager), nameof(RaceDataManager.GetKingdomLevel))]
    public class GetKingdomLevelPatch
    {
        public static void Postfix(ref int __result)
        {
            if (!ArchipelagoClient.IsConnected()) return;
            
            __result = KingdomManager.Level;
        }
    }
}