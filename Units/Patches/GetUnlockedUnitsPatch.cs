using System;
using System.Collections.Generic;
using HarmonyLib;
using SuperFantasyKingdom;

namespace SfkArchipelago.Units.Patches
{
    [HarmonyPatch(typeof(UnlockedUnitsManager), nameof(UnlockedUnitsManager.GetUnlocked))]
    public class GetUnlockedUnitsPatch
    {
        public static void Postfix(ref List<string> __result)
        {
            if (!ArchipelagoClient.IsConnected()) return;

            var units = new List<string>();
            foreach (var unit in (Unit[])Enum.GetValues(typeof(Unit)))
                if (UnitManager.IsUnlocked(unit))
                    units.Add(Enum.GetName(typeof(Unit), unit));

            __result = units;
        }
    }
}