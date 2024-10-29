using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ShowRealTime
{
    public static class DialogBoxGreen
    {
        public static Texture2D Sheet => Game1.content.Load<Texture2D>("LooseSprites\\DialogBoxGreen");
        public static readonly Rectangle Sprite = new Rectangle(15, 15, 160, 160);
    }


    public static class UIHelper
    {
        public static bool IsRenderingNormally()
        {
            bool[] conditions =
            {
                !Game1.game1.takingMapScreenshot,
                !Game1.eventUp,
                !Game1.viewportFreeze,
                !Game1.freezeControls,
                Game1.viewportHold <= 0,
                Game1.displayHUD
            };

            return conditions.All(condition => condition);
        }
    }

}
