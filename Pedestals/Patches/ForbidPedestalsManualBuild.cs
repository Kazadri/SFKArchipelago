using HarmonyLib;
using SuperFantasyKingdom.Buildings;
using SuperFantasyKingdom.Clickable;

namespace SfkArchipelago.Pedestals.Patches
{
    [HarmonyPatch(typeof(ClickableSprite), nameof(ClickableSprite.OnClick))]
    public static class ForbidPedestalsManualBuildPatch
    {
        private static bool Prefix(ClickableSprite __instance)
        {
            //TODO: Maybe open a dialogue explaining why the player can't do that.
            return !(__instance is BuildingSpotWorld instance && BuildingSize.WorldSmall == instance.size);
        }
    }
}