using System;
using System.Collections.Generic;
using HarmonyLib;
using SuperFantasyKingdom;

namespace SfkArchipelago.Relics.Patches
{
    [HarmonyPatch(typeof(RaceDataManager), nameof(RaceDataManager.CheckForRelicsWonFirstTime))]
    public class CheckForRelicsWonFirstTimePatch
    {
        public static void Postfix(bool save, List<string> __result)
        {
            if (!ArchipelagoClient.IsConnected()) return;

            foreach (var name in __result)
            {
                if (!Enum.TryParse(typeof(Relic), name, out var relic)) continue;

                RelicManager.SendRelic((Relic)relic);
            }
        }
    }
}