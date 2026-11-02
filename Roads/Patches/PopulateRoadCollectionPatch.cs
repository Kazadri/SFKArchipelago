using System;
using HarmonyLib;
using SuperFantasyKingdom;

namespace SfkArchipelago.Roads.Patches
{
    [HarmonyPatch(typeof(RoadBuilder), "Awake")]
    public class PopulateRoadCollectionPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Road[] ___roadCollection)
        {
            try
            {
                RoadUnlocks.SetOrder(RoadGroup.Overland, RoadSorter.Compute(___roadCollection, RoadGroup.Overland));
                RoadUnlocks.SetOrder(RoadGroup.City, RoadSorter.Compute(___roadCollection, RoadGroup.City));
            }
            catch (Exception e)
            {
                Plugin.Log.LogError(e);
            }
        }
    }
}