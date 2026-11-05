using System;
using Archipelago.MultiClient.Net.Models;
using SuperFantasyKingdom;

namespace SfkArchipelago.Resources
{
    internal static class ResourceManager
    {
        internal static void Handle(ItemInfo item)
        {
            var type = NameToType(item.ItemName);
            if (null == type) return;

            AddResource((ResourceType)type);
            
            ArchipelagoClient.AddNotification(
                $"(Filler) Resource received : {Enum.GetName(typeof(ResourceType), type)}");
            Plugin.Log.LogInfo($"(Filler) Resource received : {Enum.GetName(typeof(ResourceType), type)}");
        }

        public static ResourceType? NameToType(string resourceName)
        {
            if (!Enum.TryParse(typeof(ResourceType), resourceName, out var type))
            {
                Plugin.Log.LogError($"ResourceName {resourceName} is not a valid resource type");

                return null;
            }

            return (ResourceType)type;
        }

        private static void AddResource(ResourceType resourceType)
        {
            var instance = SuperFantasyKingdom.ResourceManager.Instance;

            if (null == instance)
            {
                Plugin.Log.LogWarning("ResourceManager is not yet instanced");
                return;
            }

            instance.AddResource(resourceType, 1);
        }
    }
}