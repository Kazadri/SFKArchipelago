using System;
using HarmonyLib;
using SuperFantasyKingdom;

namespace SfkArchipelago
{
    [HarmonyPatch(typeof(TutorialManager), nameof(TutorialManager.GetTasks))]
    public static class GetTasksPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref Tutorial[] __result)
        {
            if (!ArchipelagoClient.IsConnected()) return;
            
            Plugin.Log.LogInfo("Override GetTasks.Postfix");
            __result = Array.Empty<Tutorial>();
        }
    }
}