using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using SuperFantasyKingdom;
using SuperFantasyKingdom.Buildings;
using System;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using UnityEngine;

namespace SfkArchipelago
{
    [BepInPlugin("kazadri.sfk.archipelago", "SFK Archipelago", "0.0.1")]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource Log;
        
        private void Awake()
        {
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            
            Log = Logger;
            Logger.LogInfo("Start loading Kazadri SFK Archipelago");
            
            new Harmony("kazadri.sfk.archipelago").PatchAll();
            Logger.LogInfo("Patches Harmony applied");
            

            CityManager.OnBuildingBuilt += HandleBuildingBuilt;
            DaytimeManager.OnMorningStart += OnMorningStart;
            
            Logger.LogInfo("Plugin loaded.");
            
            ConnectToArchipelago();
        }

        private void OnDestroy()
        {
            CityManager.OnBuildingBuilt -= HandleBuildingBuilt;
            DaytimeManager.OnMorningStart -= OnMorningStart;
            
            Logger.LogInfo("Plugin destroyed.");
        }
        
        private void HandleBuildingBuilt(BuildingCity building, bool flag)
        {
            try
            {
                if (flag) return;
                
                var type = building.GetBuildingType();
                Logger.LogInfo($"Construction : {type} ({(int)type})");
                ArchipelagoClient.SendBuildingCheck(type);
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }
        
        private void OnMorningStart(int day)
        {
            try
            {
                Logger.LogInfo($"OnMorningStart: {day}");
                if (day >= DaytimeManager.Instance.GetDayLimit()) ArchipelagoClient.CompleteGoal();
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }

        private void ConnectToArchipelago()
        {
            try
            {
                ArchipelagoClient.Connect("localhost", 38281, "Kazadri");
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }
    }
}