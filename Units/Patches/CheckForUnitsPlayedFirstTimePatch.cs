using System;
using System.Collections.Generic;
using HarmonyLib;
using SuperFantasyKingdom;

namespace SfkArchipelago.Units.Patches
{
    [HarmonyPatch(typeof(RaceDataManager), nameof(RaceDataManager.CheckForUnitsPlayedFirstTime))]
    public class CheckForUnitsPlayedFirstTimePatch
    {
        public static void Postfix(bool save, List<string> __result)
        {
            Plugin.Log.LogInfo($"CheckForUnitsPlayedFirstTime called with {save}");

            foreach (var name in __result)
            {
                if (!Enum.TryParse(typeof(Unit), name, out object unit))
                {
                    return; // Hero are considered as a unit. TODO
                }
                
                UnitManager.SendUnit((Unit)unit);
            }
        }
    }
}