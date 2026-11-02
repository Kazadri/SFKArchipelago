using SfkArchipelago.Races;

namespace SfkArchipelago.Pedestals
{
    public class Pedestal
    {
        public Pedestal(string name, int offset, Race race)
        {
            Name = name;
            Offset = offset;
            Race = race;
        }
        
        public string Name;
        public int Offset;
        public Race Race;
    }
}