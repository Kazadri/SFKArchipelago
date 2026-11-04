using HarmonyLib;
using SuperFantasyKingdom;

namespace SfkArchipelago.Witches.Patches
{
    [HarmonyPatch(typeof(ResourceManager), nameof(ResourceManager.GetOrbs))]
    public class GetOrbsPatch
    {
        public static void Postfix(ref int __result)
        {
            __result = ArchipelagoClient.WitchLevel();
        }
    }
}