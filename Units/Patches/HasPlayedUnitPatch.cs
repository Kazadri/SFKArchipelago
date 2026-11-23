using System;
using HarmonyLib;
using SuperFantasyKingdom;
using Unit = SfkArchipelago.Units.Unit;
using UnitManager = SfkArchipelago.Units.UnitManager;

namespace SfkArchipelago
{
    [HarmonyPatch(typeof(RaceDataManager), nameof(RaceDataManager.HasPlayedUnit))]
    public class HasPlayedUnitPatch
    {
        public static void Postfix(string key, ref bool __result)
        {
            if (!ArchipelagoClient.IsConnected()) return;

            if (!Enum.TryParse(typeof(Unit), key, out var unit)) return; // Hero are also checked with this method.

            if (!UnitManager.IsManaged((Unit)unit)) return;

            __result = UnitManager.IsUnitSent((Unit)unit);
        }
    }
}