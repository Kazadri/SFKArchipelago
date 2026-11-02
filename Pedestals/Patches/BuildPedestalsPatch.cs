using HarmonyLib;
using SuperFantasyKingdom;

namespace SfkArchipelago.Pedestals.Patches
{
    [HarmonyPatch(typeof(RaceDataManager), nameof(RaceDataManager.Magic))]
    public class BuildPedestalsPatch
    {
        private static void Prefix() => PedestalUnlocks.InMagic = true;

        private static void Postfix()
        {
            if (!ArchipelagoClient.IsConnected()) return;
            
            PedestalUnlocks.InMagic = false;
            PedestalUnlocks.SpawnAllUnlocked();
        }

        private static void Finalizer() => PedestalUnlocks.InMagic = false;
    }
}