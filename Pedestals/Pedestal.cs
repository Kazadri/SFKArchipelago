using SfkArchipelago.Races;

namespace SfkArchipelago.Pedestals
{
    public class Pedestal
    {
        public string Name;
        public int Offset;
        public Race Race;

        public Pedestal(string name, int offset, Race race)
        {
            Name = name;
            Offset = offset;
            Race = race;
        }
    }
}