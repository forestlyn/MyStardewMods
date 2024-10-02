using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

    public static class Icons
    {
        /// <summary>The sprite sheet containing the icon sprites.</summary>
        public static Texture2D Sheet => Game1.mouseCursors;

        /// <summary>A down arrow.</summary>
        public static readonly Rectangle DownArrow = new(12, 76, 40, 44);

        /// <summary>An up arrow.</summary>
        public static readonly Rectangle UpArrow = new(76, 72, 40, 44);
    }

    public class MyMenu : IClickableMenu
    {
        private SpriteFont font = Game1.dialogueFont;
        private List<List<string>>? contentList;

        private readonly ClickableTextureComponent ScrollUpButton;

        /// <summary>The clickable 'scroll down' icon.</summary>
        private readonly ClickableTextureComponent ScrollDownButton;

        /// <summary>The spacing around the scroll buttons.</summary>
        private readonly int ScrollButtonGutter = 15;
        private int CurrentScroll;
        private readonly int ScrollAmount = 160;
        private int MaxScroll;

        public MyMenu()
        {
            var viewport = Game1.graphics.GraphicsDevice.Viewport;
            this.width = 1280;
            this.height = 720;
            this.xPositionOnScreen = (viewport.Width - width) / 2;
            this.yPositionOnScreen = (viewport.Height - height) / 2;
            initialize(xPositionOnScreen, yPositionOnScreen, width, height, true);
            //MyLog.Log($"width: {this.width} height:{this.height} xPositionOnScreen:{xPositionOnScreen} " +
            //    $"yPositionOnScreen:{yPositionOnScreen}", StardewModdingAPI.LogLevel.Debug);

            this.ScrollUpButton = new ClickableTextureComponent(Rectangle.Empty, Icons.Sheet, Icons.UpArrow, 1);
            this.ScrollDownButton = new ClickableTextureComponent(Rectangle.Empty, Icons.Sheet, Icons.DownArrow, 1);
            this.UpdateLayout();
        }

        private void UpdateLayout()
        {
            // update up/down buttons
            int x = this.xPositionOnScreen;
            int y = this.yPositionOnScreen;
            int gutter = this.ScrollButtonGutter;
            float contentHeight = this.height - gutter * 5;
            float contentWidth = this.width - gutter * 7;

            this.ScrollUpButton.bounds = new Rectangle((int)(x + contentWidth), (int)(y + contentHeight - Icons.UpArrow.Height - gutter - Icons.DownArrow.Height), Icons.UpArrow.Height, Icons.UpArrow.Width);
            this.ScrollDownButton.bounds = new Rectangle((int)(x + contentWidth), (int)(y + contentHeight - Icons.DownArrow.Height), Icons.DownArrow.Height, Icons.DownArrow.Width);
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
        #region IClickableMenu
        public override void receiveScrollWheelAction(int direction)
        {
            if (direction > 0)    // positive number scrolls content up
                this.ScrollUp();
            else
                this.ScrollDown();
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (!this.isWithinBounds(x, y))
                this.exitThisMenu();

            // scroll up or down
            else if (this.ScrollUpButton.containsPoint(x, y))
                this.ScrollUp();
            else if (this.ScrollDownButton.containsPoint(x, y))
                this.ScrollDown();
        }

        /// <summary>The method called when the player presses a controller button.</summary>
        /// <param name="button">The controller button pressed.</param>
        public override void receiveGamePadButton(Buttons button)
        {
            switch (button)
            {
                // left click
                case Buttons.A:
                    Point p = Game1.getMousePosition();
                    this.receiveLeftClick(p.X, p.Y);
                    break;

                // exit
                case Buttons.B:
                    this.exitThisMenu();
                    break;

                // scroll up
                case Buttons.RightThumbstickUp:
                    this.ScrollUp();
                    break;

                // scroll down
                case Buttons.RightThumbstickDown:
                    this.ScrollDown();
                    break;
            }
        }
        public void ScrollUp(int? amount = null)
        {
            this.CurrentScroll -= amount ?? this.ScrollAmount;
        }

        /// <inheritdoc />
        public void ScrollDown(int? amount = null)
        {
            this.CurrentScroll += amount ?? this.ScrollAmount;
        }
        #endregion


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

                    // scroll view
                    this.CurrentScroll = Math.Max(0, this.CurrentScroll); // don't scroll past top
                    this.CurrentScroll = Math.Min(this.MaxScroll, this.CurrentScroll); // don't scroll past bottom
                    topOffset -= this.CurrentScroll; // scrolled down == move text up

                    if (contentList != null)
                    {
                        x += gutter;
                        int maxYDelta = 0;
                        foreach (var contentStrs in contentList)
                        {
                            foreach (var str in contentStrs)
                            {
                                contentBatch.DrawString(font, str, new Vector2(x + leftOffset, y + topOffset), Color.Black);
                                y += gutter;

                            }
                            maxYDelta = Math.Max(maxYDelta, y - yPositionOnScreen);
                            x += 250;
                            y = yPositionOnScreen;
                        }
                        topOffset += maxYDelta;
                    }

                    // update max scroll
                    this.MaxScroll = Math.Max(0, (int)(topOffset - contentHeight + this.CurrentScroll));

                    //MyLog.Log($"MaxScroll:{this.MaxScroll} CurrentScroll:{CurrentScroll}", LogLevel.Debug);

                    // draw scroll icons
                    if (this.MaxScroll > 0 && this.CurrentScroll > 0)
                        this.ScrollUpButton.draw(contentBatch);
                    if (this.MaxScroll > 0 && this.CurrentScroll < this.MaxScroll)
                        this.ScrollDownButton.draw(contentBatch);

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
