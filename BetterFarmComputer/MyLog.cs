using StardewModdingAPI;

namespace BetterFarmComputer
{
    internal static class MyLog
    {
        public static IMonitor? Monitor;
        public static void Log(string message, LogLevel level = LogLevel.Debug)
        {
            if (Monitor != null)
                Monitor.Log(message, level);
        }
    }
}
