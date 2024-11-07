using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace Utility
{

    public interface MySprite
    {
        public Texture2D Sheet {get;}
        public Rectangle Sprite { get; }
    }


    public class DialogBoxGreen : MySprite
    {
        public Texture2D Sheet => Game1.content.Load<Texture2D>("LooseSprites\\DialogBoxGreen");
        public Rectangle Sprite => new Rectangle(15, 15, 160, 160);
    }

    public class BoardGameBorder: MySprite
    {
        public Texture2D Sheet => Game1.content.Load<Texture2D>("LooseSprites\\boardGameBorder");
        public Rectangle Sprite => new Rectangle(0, 0, 138, 74);
    }


    public class TimeBorder : MySprite
    {
        public Texture2D Sheet => Game1.content.Load<Texture2D>("LooseSprites\\Cursors.zh-CN");
        public Rectangle Sprite => new Rectangle(332, 431, 72, 41);
    }

    public class TimeBorderWithoutCycle : MySprite
    {
        public Texture2D Sheet => Game1.content.Load<Texture2D>("LooseSprites\\Cursors.zh-CN");
        public Rectangle Sprite => new Rectangle(357, 431, 46, 40);
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

        public static Point GetGraphicSize()
        {
            return Game1.graphics.GraphicsDevice.Viewport.Bounds.Size;
        }
    }

}
