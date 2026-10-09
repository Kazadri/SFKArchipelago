using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using SuperFantasyKingdom;
using SuperFantasyKingdom.Buildings;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SfkArchipelago
{
    [BepInPlugin("kazadri.sfk.archipelago", "SFK Archipelago", "0.0.1")]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource Log;
        internal static bool RunVisible = false;
        
        private void Awake()
        {
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            
            Log = Logger;
            Logger.LogInfo("Start loading Kazadri SFK Archipelago");
            
            // Harmony patches
            new Harmony("kazadri.sfk.archipelago").PatchAll();
            Logger.LogInfo("Patches Harmony applied");
            
            // Events
            CityManager.OnBuildingBuilt += HandleBuildingBuilt;
            DaytimeManager.OnMorningStart += OnMorningStart;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            
            Logger.LogInfo("Plugin loaded.");
            
            ConnectToArchipelago();
        }
        
        private void OnDestroy()
        {
            CityManager.OnBuildingBuilt -= HandleBuildingBuilt;
            DaytimeManager.OnMorningStart -= OnMorningStart;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            
            Logger.LogInfo("Plugin destroyed.");
        }

        private void Update()
        {
            try
            {
                HandleNotification();
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }
        
        private void HandleNotification()
        {
            // AlertManager is not yet instanced, we cannot send notification.
            if (AlertManager.Instance == null) return;

            // I can only show notification in a run yet, so display before is useless.
            if (!RunVisible) return;
            
            while (ArchipelagoClient.Notifications.TryDequeue(out var message))
            {
                AlertManager.Instance.DisplayMessage("Archipelago notification", message);
                Logger.LogInfo($"[Notification] {message}");
            }
        }

        private void HandleBuildingBuilt(BuildingCity building, bool flag)
        {
            try
            {
                if (flag) return;
                
                var type = building.GetBuildingType();
                Logger.LogInfo($"Building : {type} ({(int)type}) is built");
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
                RunVisible  = true;
                if (day >= DaytimeManager.Instance.GetDayLimit())
                {
                    ArchipelagoClient.CompleteGoal();
                    return;
                }

                if (day > 1)
                {
                    ArchipelagoClient.SendDailyCheck(day-1);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }

        private static void OnSceneUnloaded(Scene scene)
        {
            if ("GameScene" == scene.name)
            {
                RunVisible = false;
            }
        }

        private void ConnectToArchipelago()
        {
            try
            {
                ArchipelagoClient.StartConnect(
                    Config.Bind("Archipelago", "Host", "localhost", "Archipelago server address").Value,
                    Config.Bind("Archipelago", "Port", 38281, "Port number").Value,
                    Config.Bind("Archipelago", "SlotName", "Player1", "Slot name (field 'name' of the YAML)").Value,
                    Config.Bind("Archipelago", "Password", "", "Archipelago password (leave blank if there are none)").Value
                );
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }
    }
}