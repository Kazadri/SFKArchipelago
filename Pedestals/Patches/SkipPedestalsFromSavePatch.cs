using HarmonyLib;
using SuperFantasyKingdom;

namespace SfkArchipelago.Pedestals.Patches
{
    [HarmonyPatch(typeof(WorldManager), nameof(WorldManager.SpawnRebuild))]
    public class SkipPedestalsFromSavePatch
    {
        private static bool Prefix(bool fromSave)
        {
            if (!ArchipelagoClient.IsConnected()) return true;

            return !(PedestalManager.InMagic && fromSave);
        }
    }
}