using HarmonyLib;
using SuperFantasyKingdom.Buildings;

namespace SfkArchipelago.Buildings.Patches

{
    [HarmonyPatch(typeof(Building), nameof(Building.CanBeUpgraded))]
    public class CanUpgradePatch
    {
        [HarmonyPostfix]
        public static void Postfix(Building __instance, ref bool __result)
        {
            if (!ArchipelagoClient.IsConnected()) return;

            var type = __instance.GetBuildingType();

            if (!UpgradeManager.IsManaged(type)) return;

            __result = UpgradeManager.IsUnlocked(type);
        }
    }
}