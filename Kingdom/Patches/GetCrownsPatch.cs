using HarmonyLib;
using SuperFantasyKingdom;

namespace SfkArchipelago.Kingdom.Patches
{
    [HarmonyPatch(typeof(RaceDataManager), nameof(RaceDataManager.GetCrowns))]
    public class GetCrownsPatch
    {
        public static void Postfix(ref int __result)
        {
            __result = 0; // Avoid the leveling to appears.
        }
    }
}