using System.Collections.Generic;
using HarmonyLib;
using SuperFantasyKingdom;

namespace SfkArchipelago.Heroes.Patches
{
    /**
     * Always return an empty list to only show unlocked heroes
     */
    [HarmonyPatch(typeof(RaceDataManager), nameof(RaceDataManager.GetFoundHeroes))]
    public class GetFoundHeroesPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref List<string> __result)
        {
            if (!ArchipelagoClient.IsConnected()) return;

            var heroes = new List<string>();

            __result = heroes;
        }
    }
}