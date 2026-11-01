using HarmonyLib;
using SuperFantasyKingdom.Buildings;
using SuperFantasyKingdom.UI;
using TMPro;
using UnityEngine;

namespace SfkArchipelago.Buildings.Patches
{
    [HarmonyPatch(typeof(UIBuildingOverlayElementUpgrade), nameof(UIBuildingOverlayElementUpgrade.Draw))]
    public class DisableUpgradeButtonPatch
    {
        [HarmonyPostfix]
        public static void PostFix(
            Building building,
            UIBuildingOverlayElementUpgrade __instance,
            int ___amountOfUpgrades
        ) {
            if (!ArchipelagoClient.IsConnected()) return;
            
            var type = building.GetBuildingType();
            if (!ArchipelagoClient.IsUpgradeManaged(type)) return;
            if (ArchipelagoClient.IsUpgradeUnlocked(type)) return;
            
            if (building.GetUpgradeAmount() >= ___amountOfUpgrades) return;
            
            var selection = Traverse.Create(__instance)
                .Method("GetOverlaySelection")
                .GetValue<Transform>();
            if (selection == null || selection.childCount == 0) return;
            
            var root = selection.GetChild(selection.childCount - 1);
            
            var message = root.Find("Upgrade/Message");
            if (message == null) return;
            
            message.gameObject.SetActive(true);
            var text = message.GetComponent<TextMeshProUGUI>();
            text.text = "This upgrade has not been unlocked yet (Archipelago).";
        }
    }
}