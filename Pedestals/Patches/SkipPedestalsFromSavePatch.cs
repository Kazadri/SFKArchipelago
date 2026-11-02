using HarmonyLib;
using SuperFantasyKingdom;

namespace SfkArchipelago.Pedestals.Patches
{
    [HarmonyPatch(typeof(WorldManager), nameof(WorldManager.SpawnRebuild))]
    public class SkipPedestalsFromSavePatch
    {
        private static bool Prefix(bool fromSave)
        {
            return !(PedestalUnlocks.InMagic && fromSave);
        }
    }
}