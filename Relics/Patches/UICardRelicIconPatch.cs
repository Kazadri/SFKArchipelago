using HarmonyLib;
using SfkArchipelago.Assets;
using SuperFantasyKingdom.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SfkArchipelago.Relics.Patches
{
    [HarmonyPatch(typeof(UICardRelic), "Init")]
    public class UICardRelicIconPatch
    {
        public static void Postfix(UICardRelic __instance, GameObject ___neverUsed)
        {
            if (!ArchipelagoClient.IsConnected()) return;
            if (___neverUsed == null) return;

            if (___neverUsed.transform.Find("ArchipelagoIcon") != null) return;

            var text = ___neverUsed.GetComponentInChildren<Image>(true);
            if (text != null) text.enabled = false;

            var go = new GameObject("ArchipelagoIcon", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(___neverUsed.transform, false);

            var img = go.GetComponent<Image>();
            img.sprite = ModAssets.ArchipelagoIcon;
            img.raycastTarget = false;
            img.preserveAspect = true;

            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}