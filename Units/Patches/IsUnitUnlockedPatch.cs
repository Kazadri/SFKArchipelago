using System;
using HarmonyLib;
using SuperFantasyKingdom;

namespace SfkArchipelago.Units.Patches
{
    [HarmonyPatch(typeof(UnlockedUnitsManager), nameof(UnlockedUnitsManager.IsUnlocked))]
    public class IsUnitUnlockedPatch
    {
        public static void Postfix(string entity, ref bool __result)
        {
            if (!ArchipelagoClient.IsConnected()) return;
            
            Enum.TryParse(typeof(Unit),  entity, out var unit);
            __result = UnitManager.IsUnlocked((Unit)unit);
        }
    }
}