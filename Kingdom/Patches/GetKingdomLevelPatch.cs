using HarmonyLib;
using SuperFantasyKingdom;

namespace SfkArchipelago.Kingdom.Patches
{
    [HarmonyPatch(typeof(RaceDataManager), nameof(RaceDataManager.GetKingdomLevel))]
    public class GetKingdomLevelPatch
    {
        public static void Postfix(ref int __result)
        {
            __result = KingdomManager.Level;
        }
    }
}