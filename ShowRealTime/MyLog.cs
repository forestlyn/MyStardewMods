using StardewModdingAPI;

namespace ShowRealTime
{
    internal static class MyLog
    {
        public static IMonitor? Monitor;
        public static void Log(string message, LogLevel level = LogLevel.Trace)
        {
            if (Monitor != null)
                Monitor.Log(message, level);
        }
    }
}
