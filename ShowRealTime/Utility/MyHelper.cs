using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
