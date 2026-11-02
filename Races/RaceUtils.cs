using System;

namespace SfkArchipelago.Races
{
    public static class RaceUtils
    {
        public static bool TryFromGame(SuperFantasyKingdom.Races gameRace, out Race race)
        {
            switch (gameRace)
            {
                case SuperFantasyKingdom.Races.Human:
                    race = Race.Human;
                    return true;
                case SuperFantasyKingdom.Races.Undead:
                    race = Race.Undead;
                    return true;
                default:
                    race = default;
                    return false;
            }
        }
        
        public static Race FromGame(SuperFantasyKingdom.Races gameRace) => gameRace switch
        {
            SuperFantasyKingdom.Races.Human => Race.Human,
            SuperFantasyKingdom.Races.Undead => Race.Undead,
            _ => throw new ArgumentOutOfRangeException(nameof(gameRace), gameRace, "Race not playable")
        };
        
        public static SuperFantasyKingdom.Races ToGame(this Race race) => race switch
        {
            Race.Human => SuperFantasyKingdom.Races.Human,
            Race.Undead => SuperFantasyKingdom.Races.Undead,
            _ => throw new ArgumentOutOfRangeException(nameof(race))
        };

        public static Race CurrentRace()
        {
            return FromGame(MainManager.Instance.GetSelectedKingdom());
        }
    }
}