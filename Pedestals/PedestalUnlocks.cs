using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using SfkArchipelago.Races;
using SuperFantasyKingdom;

namespace SfkArchipelago.Pedestals
{
    public class PedestalUnlocks
    {
        public static bool InMagic;
        
        static readonly AccessTools.FieldRef<SuperFantasyKingdom.Clickable.BuildingSpotWorld, ResourceType> ResourceRef =
            AccessTools.FieldRefAccess<SuperFantasyKingdom.Clickable.BuildingSpotWorld, ResourceType>("resource");

        public static readonly List<Pedestal> All = new List<Pedestal>
        {
            new Pedestal("Berry1", 0, Race.Human),
            new Pedestal("Berry2", 1, Race.Human),
            new Pedestal("Berry3", 2, Race.Human),
            new Pedestal("Berry4", 3, Race.Human),
            new Pedestal("Gold1", 4, Race.Human),
            new Pedestal("Gold2", 5, Race.Human),
            new Pedestal("Gold3", 6, Race.Human),
            new Pedestal("Gold4", 7, Race.Human),
            new Pedestal("Gold5", 8, Race.Human),
            new Pedestal("Stone1", 9, Race.Human),
            new Pedestal("Stone2", 10, Race.Human),
            new Pedestal("Stone3", 11, Race.Human),
            new Pedestal("Stone4", 12, Race.Human),
            new Pedestal("Stone5", 13, Race.Human),
            new Pedestal("Stone6", 14, Race.Human),
            new Pedestal("Stone7", 15, Race.Human),
            new Pedestal("Wheat1", 16, Race.Human),
            new Pedestal("Wheat2", 17, Race.Human),
            new Pedestal("Wheat3", 18, Race.Human),
            new Pedestal("Wheat4", 19, Race.Human),
            new Pedestal("Wheat5", 20, Race.Human),
            new Pedestal("Wheat6", 21, Race.Human),
            new Pedestal("Wood1", 22, Race.Human),
            new Pedestal("Wood2", 23, Race.Human),
            new Pedestal("Wood3", 24, Race.Human),
            new Pedestal("Wood4", 25, Race.Human),
            new Pedestal("Wood5", 26, Race.Human),
            new Pedestal("Wood6", 27, Race.Human),
            new Pedestal("Wood7", 28, Race.Human),
        };
        
        static readonly Dictionary<(Race, string), Pedestal> ByKey =
            All.ToDictionary(p => (p.Race, p.Name));
        
        public static bool TryGet(Race race, string name, out Pedestal p)
            => ByKey.TryGetValue((race, name), out p);

        public static void SpawnAllUnlocked()
        {
            foreach (var spot in UnityEngine.Object.FindObjectsOfType<SuperFantasyKingdom.Clickable.BuildingSpotWorld>())
            {
                if (TryGet(RaceUtils.CurrentRace(), spot.name, out var p) && ArchipelagoClient.IsPedestalUnlocked(p))
                    WorldManager.Instance.SpawnRebuild(spot.transform.position, ResourceRef(spot), fromSave: true);
            }
        }
    }
}