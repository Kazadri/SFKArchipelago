using HarmonyLib;
using SuperFantasyKingdom;
using SuperFantasyKingdom.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SfkArchipelago.Buildings.Patches
{
    [HarmonyPatch(typeof(UIBuildingOverlayElementCityEvent), nameof(UIBuildingOverlayElementCityEvent.Draw))]
    public class CanUpgradeCastlePatch
    {
        [HarmonyPostfix]
        public static void Postfix(GameObject __result, CityEvent ___eventToTrigger)
        {
            if (__result == null) return;
            if (!ArchipelagoClient.IsConnected()) return;

            if (!CastleManager.IsManaged(___eventToTrigger)) return;
            if (CastleManager.IsUnlocked(___eventToTrigger)) return;

            var button = __result.GetComponent<Button>();
            if (button != null) button.interactable = false;

            var group = __result.GetComponent<CanvasGroup>();
            if (group != null) group.alpha = 0.5f;

            var description = __result.transform.Find("TextDescription")?.GetComponent<TextMeshProUGUI>();
            if (description != null)
                description.text += "\n<color=#ff6666>Not unlocked yet (Archipelago).</color>";
        }
    }
}