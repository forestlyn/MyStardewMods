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
}
