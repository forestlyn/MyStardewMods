using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace ShowRealTime
{
    public class TimeMenu : IClickableMenu, IDisposable
    {
        private SpriteFont font = Game1.dialogueFont;

        private IModHelper helper;

        /// <summary>
        /// 24进制时间
        /// </summary>
        private string time24_String = "";
        /// <summary>
        /// 12进制时间
        /// </summary>
        private string time12_String = "";
        /// <summary>
        /// 日期 eg:1970/1/1
        /// </summary>
        private string date = "";

        private Vector2 position;

        private ModConfig config;

        public TimeMenu(IModHelper modHelper, ModConfig config, int x, int y, int width, int height) : base(x, y, width, height, false)
        {
            helper = modHelper;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.Display.RenderingHud += OnRenderingHud;

            this.config = config;
            this.position = new Vector2((float)this.xPositionOnScreen, (float)this.yPositionOnScreen);
            this.width = width;
            this.height = height;
            font = Game1.dialogueFont;
        }

        private void OnRenderingHud(object? sender, RenderingHudEventArgs e)
        {
            if (UIHelper.IsRenderingNormally())
            {
                Draw(Game1.spriteBatch);
            }
        }


        public void Dispose()
        {
            helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
            helper.Events.Display.RenderingHud -= OnRenderingHud;
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            DateTime now = DateTime.Now;
            time24_String = now.ToString("HH:mm");
            time12_String = now.ToString("hh:mm tt");
            date = now.ToString("yyyy/M/d");
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

        public bool ShowDate { get; internal set; }
        public bool Is24Hour { get; internal set; }

        public void Draw(SpriteBatch b)
        {
            float xScale = 2f;
            float yScale = 2f;
            float x = this.xPositionOnScreen;
            float y = this.yPositionOnScreen;
            const int gutter = 25;
            float leftOffset = gutter;
            float topOffset = gutter;
            float contentWidth = this.width * xScale - gutter * 2;
            float contentHeight = this.height * yScale - gutter * 2;

            using (SpriteBatch backgroundBatch = new SpriteBatch(Game1.graphics.GraphicsDevice))
            {
                backgroundBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, 
                    SamplerState.PointClamp);
                backgroundBatch.Draw(BoardGameBorder.Sheet, new Vector2(x, y), 
                    BoardGameBorder.Sprite, Color.White, 0, Vector2.Zero, 
                    new Vector2(xScale, yScale), SpriteEffects.None, 0);
                backgroundBatch.End();
            }

            using (SpriteBatch contentBatch = new SpriteBatch(Game1.graphics.GraphicsDevice))
            {
                GraphicsDevice device = Game1.graphics.GraphicsDevice;
                Rectangle prevScissorRectangle = device.ScissorRectangle;
                try
                {
                    device.ScissorRectangle = new Rectangle((int)(x + gutter), 
                        (int)(y + gutter), (int)contentWidth, (int)contentHeight);
                    contentBatch.Begin(SpriteSortMode.Deferred, this.ContentBlendState, 
                        SamplerState.PointClamp, null, new RasterizerState { ScissorTestEnable = true });
                    contentBatch.DrawString(font, config.use_24_hour_Clock ? time24_String : time12_String, 
                        new Vector2(x + leftOffset, y + topOffset), Color.Black);
                    y += leftOffset * 2;
                    if (config.showDate)
                        contentBatch.DrawString(font, date, new Vector2(x + leftOffset, y + topOffset), Color.Black);
                    contentBatch.End();
                }
                catch (Exception ex)
                {
                    MyLog.Log(ex.ToString(), LogLevel.Debug);
                }
                finally
                {
                    device.ScissorRectangle = prevScissorRectangle;
                }
            }
            this.drawMouse(Game1.spriteBatch);
        }

        public override void leftClickHeld(int x, int y)
        {

        }
    }
}
