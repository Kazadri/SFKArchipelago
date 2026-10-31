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
            var heroes = new List<string>();

            foreach (Hero hero in (Hero[]) Enum.GetValues(typeof(Hero)))
            {
                if (ArchipelagoClient.IsHeroUnlocked(hero))
                {
                    heroes.Add(Enum.GetName(typeof(Hero), hero));
                }
            }
            
            __result = heroes;
        }
    }
    
    /**
     * Always return an empty list to only show unlocked heroes
     */
    [HarmonyPatch(typeof(RaceDataManager), nameof(RaceDataManager.GetFoundHeroes))]
    public class GetFoundHeroesPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref List<string> __result)
        {
            var heroes = new List<string>();

            __result = heroes;
        }
    }
}