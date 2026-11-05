using HarmonyLib;
using SuperFantasyKingdom;

namespace SfkArchipelago.Pedestals.Patches
{
    [HarmonyPatch(typeof(RaceDataManager), nameof(RaceDataManager.Magic))]
    public class BuildPedestalsPatch
    {
        private static void Prefix()
        {
            PedestalManager.InMagic = true;
        }

        private static void Postfix()
        {
            if (!ArchipelagoClient.IsConnected()) return;

            PedestalManager.InMagic = false;
            PedestalManager.SpawnAllUnlocked();
        }

        private static void Finalizer()
        {
            PedestalManager.InMagic = false;
        }
    }
}