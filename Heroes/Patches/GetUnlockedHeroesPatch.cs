using System;
using System.Collections.Generic;
using HarmonyLib;
using SuperFantasyKingdom;

namespace SfkArchipelago.Heroes.Patches
{
    [HarmonyPatch(typeof(RaceDataManager), nameof(RaceDataManager.GetUnlockedHeroes))]
    public class GetUnlockedHeroesPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref List<string> __result)
        {
            if (!ArchipelagoClient.IsConnected()) return;

            var heroes = new List<string>();

            foreach (var hero in (Hero[])Enum.GetValues(typeof(Hero)))
                if (ArchipelagoClient.IsHeroUnlocked(hero))
                    heroes.Add(Enum.GetName(typeof(Hero), hero));

            __result = heroes;
        }
    }
}