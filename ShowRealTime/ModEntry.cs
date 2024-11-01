using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Utility;
using xTile.Dimensions;

namespace ShowRealTime
{
    public class ModEntry : Mod
    {
        private TimeMenu timeMenu;
        private IModHelper helper;

        private ModConfig Config { get; set; }

        public event IsInMineEventHandler IsInMineEvent;
        public override void Entry(IModHelper helper)
        {
            this.helper = helper;
            this.Config = this.Helper.ReadConfig<ModConfig>();

            MyHelper.SetHelper(helper);
            MyLog.Monitor = this.Monitor;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.Player.Warped += OnWarped;
        }

        private void OnWarped(object? sender, WarpedEventArgs e)
        {
            if(e.IsLocalPlayer && CheckIsMine(e.NewLocation))
            {
                IsInMineEvent?.Invoke(true);
            }
            else
            {
                IsInMineEvent?.Invoke(false);
            }
        }

        private bool CheckIsMine(GameLocation location)
        {
            if (location.Name == "Mine"||location.Name == "SkullCave")
            {
                return true;
            }
            return false;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            InitConfig();
            timeMenu = new TimeMenu(helper, Config, new TimeBorderWithoutCycle());
            IsInMineEvent += timeMenu.IsInMine;
        }




        #region ModConfig
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
                name: () => MyHelper.GetTranslation("Use24-HourClock"),
                tooltip: () => "If enabled, the clock will use a 24-hour format (e.g. 14:30). " +
                       "If disabled, it will use a 12-hour format with AM/PM (e.g. 2:30 PM).",
                getValue: () => this.Config.use_24_hour_Clock,
                setValue: value =>
                {
                    this.Config.use_24_hour_Clock = value;
                }
            );


            for (int i = 0; i < Config.ClockNum - Config.Clocks.Count; i++)
            {
                Config.Clocks.Add(new Clock());
                //MyLog.Log($"增加了一个时钟");
            }

            for (int i = 0; i < Config.Clocks.Count; i++)
            {
                int index = i;
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => MyHelper.GetTranslation("ClockEnable").Replace("{index}",(index+1).ToString()),
                    tooltip: () => $"If enabled, the clock will remind you at the time you set.",
                    getValue: () => this.Config.Clocks[index].UseClock,
                    setValue: value =>
                    {
                        this.Config.Clocks[index].UseClock = value;
                    }
                );

                configMenu.AddNumberOption(
                    mod: this.ModManifest,
                    name: () => MyHelper.GetTranslation("Hour"),
                    getValue: () => this.Config.Clocks[index].Hour,
                    setValue: value =>
                    {
                        this.Config.Clocks[index].Hour= value;
                    },
                    min: 0,
                    max: 23,
                    interval: 1
                );

                configMenu.AddNumberOption(
                    mod: this.ModManifest,
                    name: () => MyHelper.GetTranslation("Minute"),
                    getValue: () => this.Config.Clocks[index].Minute,
                    setValue: value =>
                    {
                        this.Config.Clocks[index].Minute = value;
                    },
                    min: 0,
                    max: 59,
                    interval: 1
                );
            }

            //configMenu.AddBoolOption(
            //    mod: this.ModManifest,
            //    name: () => "Show date",
            //    tooltip: () => "If enabled, the clock will show the current date(e.g. 1970/1/1).",
            //    getValue: () => this.Config.showDate,
            //    setValue: value =>
            //    {
            //        this.Config.showDate = value;
            //    }
            //);

        }
        #endregion
    }
}