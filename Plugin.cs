using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using SfkArchipelago.Buildings;
using SfkArchipelago.Races;
using SuperFantasyKingdom;
using SuperFantasyKingdom.Buildings;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hero = SfkArchipelago.Heroes.Hero;

namespace SfkArchipelago
{
    [BepInPlugin("kazadri.sfk.archipelago", "SFK Archipelago", "0.0.1")]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource Log;
        internal static bool RunVisible;

        private void Awake()
        {
            gameObject.hideFlags = HideFlags.HideAndDontSave;

            Log = Logger;
            Logger.LogInfo("Start loading Kazadri SFK Archipelago");

            // Harmony patches
            var harmony = new Harmony("kazadri.sfk.archipelago");
            harmony.PatchAll();
            foreach (var m in harmony.GetPatchedMethods())
                Log.LogInfo($"Patched: {m.DeclaringType}.{m.Name}");

            // Events
            CityManager.OnBuildingBuilt += HandleBuildingBuilt;
            DaytimeManager.OnMorningStart += OnMorningStart;
            DaytimeManager.OnNightStart += OnNightStart;
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            Logger.LogInfo("Plugin loaded.");

            ConnectToArchipelago();
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

        private void OnDestroy()
        {
            CityManager.OnBuildingBuilt -= HandleBuildingBuilt;
            DaytimeManager.OnMorningStart -= OnMorningStart;
            DaytimeManager.OnNightStart -= OnNightStart;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;

            Logger.LogInfo("Plugin destroyed.");
        }

        private void HandleNotification()
        {
            while (ArchipelagoClient.Notifications.TryDequeue(out var message))
            {
                // AlertManager is not yet instanced, we cannot send notification.
                if (AlertManager.Instance == null) continue;

                // I can only show notification in a run yet, so display before is useless.
                if (!RunVisible) continue;

                AlertManager.Instance.DisplayMessage("Archipelago notification", message);
                Logger.LogInfo($"[Notification] {message}");
            }
        }

        /**
         * TODO: Move this somewhere else
         */
        private void HandleBuildingBuilt(BuildingCity building, bool flag)
        {
            try
            {
                if (flag) return;

                var type = building.GetBuildingType();
                Logger.LogInfo($"Building : {type} ({(int)type}) is built");
                BuildingManager.SendLocation(type);
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }

        private void OnMorningStart(int day)
        {
            RunVisible = true;
        }

        private void OnNightStart(int day)
        {
            try
            {
                Enum.TryParse(typeof(Hero), MainManager.Instance.GetSelectedHero(), out var hero);
                var race = RaceUtils.CurrentRace();

                ArchipelagoClient.SendDailyCheck((int)race * 308 + (int)hero * 28 + day);
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }

        private static void OnSceneUnloaded(Scene scene)
        {
            if ("GameScene" == scene.name) RunVisible = false;
        }

        private void ConnectToArchipelago()
        {
            try
            {
                ArchipelagoClient.StartConnect(
                    Config.Bind("Archipelago", "Host", "localhost", "Archipelago server address").Value,
                    Config.Bind("Archipelago", "Port", 38281, "Port number").Value,
                    Config.Bind("Archipelago", "SlotName", "Player1", "Slot name (field 'name' of the YAML)").Value,
                    Config.Bind("Archipelago", "Password", "", "Archipelago password (leave blank if there are none)")
                        .Value
                );
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }
    }
}