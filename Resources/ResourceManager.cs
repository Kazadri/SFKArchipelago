using System;
using SuperFantasyKingdom;

namespace SfkArchipelago.Resources
{
    public static class ResourceManager
    {
        public static ResourceType? NameToType(string resourceName)
        {
            if (!Enum.TryParse(typeof(ResourceType), resourceName, out var type))
            {
                Plugin.Log.LogError($"ResourceName {resourceName} is not a valid resource type");

                return null;
            }

            return (ResourceType)type;
        }

        public static void AddResource(ResourceType resourceType)
        {
            var instance = SuperFantasyKingdom.ResourceManager.Instance;

            if (null == instance)
            {
                Plugin.Log.LogWarning("ResourceManager is not yet instanced");
                return;
            }

            instance.AddResource(resourceType, 1);

            ArchipelagoClient.AddNotification(
                $"(Filler) Resource received : {Enum.GetName(typeof(ResourceType), resourceType)}");
            Plugin.Log.LogInfo($"(Filler) Resource received : {Enum.GetName(typeof(ResourceType), resourceType)}");
        }
    }
}