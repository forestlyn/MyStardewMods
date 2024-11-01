using StardewModdingAPI;
using StardewValley;

namespace Utility
{
    public static class MyHelper
    {
        static IModHelper? ModHelper = null;
        public static void SetHelper(IModHelper modHelper)
        {
            ModHelper = modHelper;
        }

        public static string GetTranslation(string key)
        {
            if(ModHelper == null)
                return "ModHelper is null!";
            return ModHelper.Translation.Get(key).Default("缺失翻译！");
        }


        public static string GetLocationName()
        {
            return Game1.currentLocation.Name;
        }

        
    }
}
