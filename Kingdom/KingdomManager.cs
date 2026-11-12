using System.Threading;
using Archipelago.MultiClient.Net.Models;

namespace SfkArchipelago.Kingdom
{
    internal static class KingdomManager
    {
        private static int _level;
        internal static int Level => Volatile.Read(ref _level);

        internal static void Handle(ItemInfo item)
        {
            var level = Interlocked.Increment(ref _level);
            ArchipelagoClient.AddNotification($"Kingdom level unlocked: {level}");
        }

        internal static void Reset()
        {
            Volatile.Write(ref _level, 0);
        }
    }
}