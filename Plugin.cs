using BepInEx;
using BepInEx.Configuration;
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
        internal static ConfigEntry<string> ServerHost = null!;
        internal static ConfigEntry<int> ServerPort = null!;
        internal static ConfigEntry<string> SlotName = null!;
        internal static ConfigEntry<string> Password = null!;
        
        private void Awake()
        {
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            
            ServerHost = Config.Bind("Archipelago", "Host", "localhost",
                "Archipelago server address");
            ServerPort = Config.Bind("Archipelago", "Port", 38281,
                "Port number");
            SlotName = Config.Bind("Archipelago", "SlotName", "Player1",
                "Slot name (field 'name' of the YAML)");
            Password = Config.Bind("Archipelago", "Password", "",
                "Archipelago password (leave blank if there are none)");
            
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
                ArchipelagoClient.StartConnect(ServerHost.Value, ServerPort.Value, SlotName.Value, Password.Value);
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }
    }
}