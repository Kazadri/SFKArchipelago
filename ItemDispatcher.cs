using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using SfkArchipelago.Buildings;
using SfkArchipelago.Heroes;
using SfkArchipelago.Kingdom;
using SfkArchipelago.Pedestals;
using SfkArchipelago.Resources;
using SfkArchipelago.Roads;
using SfkArchipelago.Units;
using SfkArchipelago.Witches;

namespace SfkArchipelago
{
    internal static class ItemDispatcher
    {
        internal static void OnItemReceived(ReceivedItemsHelper helper)
        {
            while (helper.Any())
                Dispatch(helper.DequeueItem());
        }

        private static void Dispatch(ItemInfo item)
        {
            switch (ItemIds.Resolve(item.ItemId))
            {
                case ItemKind.Building: BuildingManager.Handle(item); break;
                case ItemKind.Castle: CastleManager.Handle(item); break;
                case ItemKind.Filler: ResourceManager.Handle(item); break;
                case ItemKind.Hero: HeroManager.Handle(item); break;
                case ItemKind.Kingdom: KingdomManager.Handle(item); break;
                case ItemKind.Pedestal: PedestalManager.Handle(item); break;
                case ItemKind.Road: RoadManager.Handle(item); break;
                case ItemKind.Unit: UnitManager.Handle(item); break;
                case ItemKind.Upgrade: UpgradeManager.Handle(item); break;
                case ItemKind.Witch: WitchManager.Handle(item); break;
                default:
                    Plugin.Log.LogInfo($"Item received (not implemented): {item.ItemName} | id={item.ItemId}");
                    break;
            }
        }

        internal static void ResetAll()
        {
            BuildingManager.Reset();
            UpgradeManager.Reset();
            CastleManager.Reset();
            RoadManager.Reset();
            HeroManager.Reset();
            PedestalManager.Reset();
            WitchManager.Reset();
        }
    }
}