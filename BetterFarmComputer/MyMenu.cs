using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace BetterFarmComputer
{
    public static class Textbox
    {
        /// <summary>The sprite sheet containing the textbox sprites.</summary>
        public static Texture2D Sheet => Game1.content.Load<Texture2D>("LooseSprites\\textBox");
    }
    public static class Letter
    {
        /// <summary>The sprite sheet containing the letter sprites.</summary>
        public static Texture2D Sheet => Game1.content.Load<Texture2D>("LooseSprites\\letterBG");
        public static readonly Rectangle Sprite = new Rectangle(0, 0, 320, 180);
    }

    internal class MyMenu : IClickableMenu
    {
        private SpriteFont font = Game1.dialogueFont;
        private List<List<string>>? contentList;

        public MyMenu()
        {
            var viewport = Game1.graphics.GraphicsDevice.Viewport;
            this.width = 1280;
            this.height = 720;
            this.xPositionOnScreen = (viewport.Width - width) / 2;
            this.yPositionOnScreen = (viewport.Height - height) / 2;
            initialize(xPositionOnScreen, yPositionOnScreen, width, height, true);
            MyLog.Log($"width: {this.width} height:{this.height} xPositionOnScreen:{xPositionOnScreen} " +
                $"yPositionOnScreen:{yPositionOnScreen}", StardewModdingAPI.LogLevel.Debug);
        }


        private readonly BlendState ContentBlendState = new()
        {
            AlphaBlendFunction = BlendFunction.Add,
            AlphaSourceBlend = Blend.Zero,
            AlphaDestinationBlend = Blend.One,

            ColorBlendFunction = BlendFunction.Add,
            ColorSourceBlend = Blend.SourceAlpha,
            ColorDestinationBlend = Blend.InverseSourceAlpha
        };

        public void SetContentLists(List<List<string>> contents)
        {
            contentList = contents;
            //MyLog.Log(contentList.Count.ToString(), LogLevel.Debug);
            //foreach (var item in contentList)
            //{
            //    foreach (var item2 in item)
            //        MyLog.Log(item2, LogLevel.Debug);
            //}
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (playSound)
            {
                Game1.playSound(closeSound);
            }
            exitThisMenu();
        }

        private float calculateScale(float width, float height, float targetWidth, float targetHeight)
        {
            float scale = MathF.Min(targetWidth / width, targetHeight / height);
            return scale;
        }



        public override void draw(SpriteBatch b)
        {
            //base.draw(b);
            //MyLog.Log("invoke draw", StardewModdingAPI.LogLevel.Debug);

            int x = this.xPositionOnScreen;
            int y = this.yPositionOnScreen;
            const int gutter = 40;
            float leftOffset = gutter;
            float topOffset = gutter;
            float contentWidth = this.width - gutter * 2;
            float contentHeight = this.height - gutter * 2;
            int tableBorderWidth = 1;


            using (SpriteBatch backgroundBatch = new SpriteBatch(Game1.graphics.GraphicsDevice))
            {
                backgroundBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp);
                float scale = calculateScale(Letter.Sprite.Width, Letter.Sprite.Height, this.width, this.height);
                backgroundBatch.Draw(Letter.Sheet, new Vector2(x, y), Letter.Sprite, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                backgroundBatch.End();
                //MyLog.Log($"{scale} {Letter.Sprite.Width} {Letter.width} {Letter.Sprite.Height} {Letter.height}", LogLevel.Debug);
            }


            using (SpriteBatch contentBatch = new SpriteBatch(Game1.graphics.GraphicsDevice))
            {
                GraphicsDevice device = Game1.graphics.GraphicsDevice;
                Rectangle prevScissorRectangle = device.ScissorRectangle;
                try
                {
                    device.ScissorRectangle = new Rectangle(x + gutter, y + gutter, (int)contentWidth, (int)contentHeight);
                    contentBatch.Begin(SpriteSortMode.Deferred, this.ContentBlendState, SamplerState.PointClamp, null, new RasterizerState { ScissorTestEnable = true });
                    if (contentList != null)
                    {
                        x += gutter;
                        foreach (var contentStrs in contentList)
                        {
                            foreach (var str in contentStrs)
                            {
                                contentBatch.DrawString(font, str, new Vector2(x + leftOffset, y + topOffset), Color.Black);
                                y += gutter;
                            }
                            x += 250;
                            y = yPositionOnScreen;
                        }
                    }


                    contentBatch.End();
                }
                catch (Exception ex)
                {
                    MyLog.Log(ex.ToString(), LogLevel.Debug);
                }
            }
            this.drawMouse(Game1.spriteBatch);
        }
    }
}
