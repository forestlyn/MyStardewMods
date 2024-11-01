#define DEBUG_MODE

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using Utility;


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

        /// <summary>
        /// 配置
        /// </summary>
        private ModConfig config;
        /// <summary>
        /// 使用的图像
        /// </summary>
        private MySprite mySprite;

        private readonly float TimeInterval = 50f * 60;
        private readonly float ShowTime = 10f * 60;

        private float showTimer = 0f;
        private float timeIntervalTimer = 0f;
        private bool drawClockText = false;

        private bool isInMine;
        public TimeMenu(IModHelper modHelper, ModConfig config, MySprite mySprite) :
            base(0, 0, mySprite.Sprite.Width, mySprite.Sprite.Height, false)
        {
            helper = modHelper;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.Display.RenderingHud += OnRenderingHud;

            this.mySprite = mySprite;
            this.config = config;
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

            for (int i = 0; i < config.Clocks.Count; i++)
            {
                if (config.Clocks[i].UseClock)
                {
                    if (now.Hour == config.Clocks[i].Hour && now.Minute == config.Clocks[i].Minute)
                    {
                        DrawClock();
                    }
                }
            }
        }

        private void DrawClock()
        {
            showTimer += 1;
            if (showTimer < ShowTime)
            {
                if(drawClockText == false)
                {
                    Game1.playSound("drumkit6", null);
                    drawClockText = true;
                }
            }
            else
            {
                timeIntervalTimer += 1;
                drawClockText = false;
                if (timeIntervalTimer > TimeInterval)
                {
                    showTimer = 0;
                    timeIntervalTimer = 0;

                }
            }
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
            float scale = 3.5f;
            float x = this.xPositionOnScreen;
            float y = this.yPositionOnScreen;
            const int gutter = 25;
            float leftOffset = 10;
            float topOffset = 10;
            float contentWidth = this.width * scale;
            float contentHeight = this.height * scale;


            if (isInMine)
            {
                y += 100;
            }

            using (SpriteBatch backgroundBatch = new SpriteBatch(Game1.graphics.GraphicsDevice))
            {
                backgroundBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied,
                    SamplerState.PointClamp);
                backgroundBatch.Draw(mySprite.Sheet, new Vector2(x, y),
                    mySprite.Sprite, Color.White, 0, Vector2.Zero,
                    scale, SpriteEffects.None, 0);
                backgroundBatch.End();
            }

            using (SpriteBatch contentBatch = new SpriteBatch(Game1.graphics.GraphicsDevice))
            {
                GraphicsDevice device = Game1.graphics.GraphicsDevice;
                Rectangle prevScissorRectangle = device.ScissorRectangle;
                try
                {
                    device.ScissorRectangle = new Rectangle((int)(x),
                        (int)(y), (int)contentWidth, (int)contentHeight);
                    contentBatch.Begin(SpriteSortMode.Deferred, this.ContentBlendState,
                        SamplerState.PointClamp, null, new RasterizerState { ScissorTestEnable = true });
                    contentBatch.DrawString(Game1.smallFont, date, new Vector2(x + 15, y + 20), Color.Black);
                    y += 100;
                    contentBatch.DrawString(Game1.smallFont, config.use_24_hour_Clock ? time24_String : time12_String,
                        new Vector2(x + 50, y), Color.Black);
                    contentBatch.End();

                    if (drawClockText)
                    {
                        DrawText(MyHelper.GetTranslation("TimeReminder").Replace("{time}", time24_String), 5, Game1.graphics.GraphicsDevice.Viewport.Height - 30);
                    }
                    //DrawText(MyHelper.GetTranslation("TimeReminder").Replace("{time}", time24_String), 5, Game1.graphics.GraphicsDevice.Viewport.Height - 30);
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

        public void DrawText(string text, int x, int y)
        {
            IClickableMenu.drawHoverText(Game1.spriteBatch, text, Game1.dialogueFont, overrideX: x, overrideY: y);
        }

        internal void IsInMine(bool isInMine)
        {
            isInMine = true;
        }
    }
}
