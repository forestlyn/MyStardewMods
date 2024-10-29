using System;
using System.Runtime.Intrinsics.X86;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace ShowRealTime
{
    public class ModEntry : Mod
    {
        private TimeMenu timeMenu;
        private IModHelper helper;

        private ModConfig Config { get; set; }
        public override void Entry(IModHelper helper)
        {
            this.helper = helper;
            this.Config = this.Helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            InitConfig();
            timeMenu = new TimeMenu(helper, Config, BoardGameBorder.Sprite.X, BoardGameBorder.Sprite.Y,
BoardGameBorder.Sprite.Width, BoardGameBorder.Sprite.Height);
        }

        private void InitConfig()
        {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
            {
                //MyLog.Log("Can't find the Generic Mod Config Menu API. Make sure it's installed.", LogLevel.Error);
                return;
            }


            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );

            // add options
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Use 24 - hour clock",
                tooltip: () => "If enabled, the clock will use a 24-hour format (e.g. 14:30). " +
                       "If disabled, it will use a 12-hour format with AM/PM (e.g. 2:30 PM).",
                getValue: () => this.Config.use_24_hour_Clock,
                setValue: value =>
                {
                    this.Config.use_24_hour_Clock = value;
                }
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Show date",
                tooltip: () => "If enabled, the clock will show the current date(e.g. 1970/1/1).",
                getValue: () => this.Config.showDate,
                setValue: value =>
                {
                    this.Config.showDate = value;
                }
            );
        }
    }
}