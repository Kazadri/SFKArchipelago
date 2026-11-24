namespace SfkArchipelago.Relics
{
    public class RelicManager
    {
        internal static bool IsManaged(Relic relic)
        {
            return true; // All relic are managed
        }

        internal static void SendRelic(Relic relic)
        {
            if (!IsManaged(relic)) return;

            ArchipelagoClient.SendLocation((int)relic + LocationIds.Relic);
        }

        internal static bool IsRelicSent(Relic relic)
        {
            return ArchipelagoClient.IsLocationChecked((int)relic + LocationIds.Relic);
        }

        internal static void Scout(Relic relic)
        {
            ArchipelagoClient.ScoutWithHint((int)relic + LocationIds.Relic);
        }
    }
}