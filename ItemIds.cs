namespace SfkArchipelago
{
    public class ItemIds
    {
        internal const long Building = 1_000_000;
        internal const long Upgrade = 1_002_000;
        internal const long Road = 1_003_000;
        internal const long Hero = 1_004_000;
        internal const long Pedestal = 1_006_000;
        internal const long Witch = 1_007_000;
        internal const long Castle = 1_008_000;
        internal const long Filler = 2_000_000;
        internal const long End = 3_000_000;

        internal static ItemKind Resolve(long id)
        {
            if (id >= End) return ItemKind.Unknown;
            if (id >= Filler) return ItemKind.Filler;
            if (id >= Castle) return ItemKind.Castle;
            if (id == Witch) return ItemKind.Witch;
            if (id >= Pedestal) return ItemKind.Pedestal;
            if (id >= Hero) return ItemKind.Hero;
            if (id >= Road) return ItemKind.Road;
            if (id >= Upgrade) return ItemKind.Upgrade;
            if (id >= Building) return ItemKind.Building;

            return ItemKind.Unknown;
        }
    }
}