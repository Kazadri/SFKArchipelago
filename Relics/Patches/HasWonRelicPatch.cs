using System;
using HarmonyLib;
using SuperFantasyKingdom;

namespace SfkArchipelago.Relics.Patches
{
    [HarmonyPatch(typeof(RaceDataManager), nameof(RaceDataManager.HasWonRelic))]
    public class HasWonRelicPatch
    {
        public static void Postfix(string key, ref bool __result)
        {
            if (!ArchipelagoClient.IsConnected()) return;

            if (!Enum.TryParse(typeof(Relic), key, out var relic)) return;

            if (!RelicManager.IsManaged((Relic)relic)) return;

            __result = RelicManager.IsRelicSent((Relic)relic);
        }
    }
}