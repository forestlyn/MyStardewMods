using BetterFarmComputer.Struct;
using GenericModConfigMenu;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using System;
using Object = StardewValley.Object;

namespace BetterFarmComputer
{
    /// <summary>The mod entry point.</summary>
    public sealed class ModEntry : Mod
    {

        private Analyse analyse = new Analyse();
        public Analyse Analyse { get { return analyse; } }

        public ModConfig Config { get; private set; }

        private int currentMenuIdx = 0;
        public int CurrentMenuIdx
        {
            get => currentMenuIdx;
            set
            {
                if (value >= 0 && value < MenuIdxCount)
                {
                    currentMenuIdx = value;
                    ShowMyMenu(CurrentMenuIdx);
                }
            }
        }
        public static int MenuIdxCount = 2;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            //helper.Events.Display.MenuChanged += this.OnDisplayMenuChanged;

            this.Config = this.Helper.ReadConfig<ModConfig>();

            helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            MyLog.Monitor = this.Monitor;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
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

            // add some config options

            configMenu.AddKeybindList(
                mod: this.ModManifest,
                getValue: () => this.Config.ToggleFarmComputer,
                setValue: value => this.Config.ToggleFarmComputer = value,
                name: () => "打开农场信息",
                tooltip: () => "打开农场信息"
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "显示农场信息",
                tooltip: () => "显示农场信息",
                getValue: () => this.Config.ShowFarm,
                setValue: value => this.Config.ShowFarm = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "显示温室信息",
                tooltip: () => "显示温室信息",
                getValue: () => this.Config.ShowGreenHouse,
                setValue: value => this.Config.ShowGreenHouse = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "显示姜岛信息",
                tooltip: () => "显示姜岛信息",
                getValue: () => this.Config.ShowIslandWest,
                setValue: value => this.Config.ShowIslandWest = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "显示其它信息",
                tooltip: () => "显示其它信息",
                getValue: () => this.Config.ShowOther,
                setValue: value => this.Config.ShowOther = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "显示树液采集器",
                tooltip: () => "显示树液采集器",
                getValue: () => this.Config.ShowTapper,
                setValue: value => this.Config.ShowTapper = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "显示小桶",
                tooltip: () => "显示小桶",
                getValue: () => this.Config.ShowKeg,
                setValue: value => this.Config.ShowKeg = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "显示蜂房",
                tooltip: () => "显示蜂房",
                getValue: () => this.Config.ShowBeeHouse,
                setValue: value => this.Config.ShowBeeHouse = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "显示地窖木桶",
                tooltip: () => "显示地窖木桶",
                getValue: () => this.Config.ShowCask,
                setValue: value => this.Config.ShowCask = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "显示罐头瓶",
                tooltip: () => "显示罐头瓶",
                getValue: () => this.Config.ShowPreserveJar,
                setValue: value => this.Config.ShowPreserveJar = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "显示蘑菇树桩",
                tooltip: () => "显示蘑菇树桩",
                getValue: () => this.Config.ShowMushroomLog,
                setValue: value => this.Config.ShowMushroomLog = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "显示烘干机",
                tooltip: () => "显示烘干机",
                getValue: () => this.Config.ShowDehydrator,
                setValue: value => this.Config.ShowDehydrator = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "显示果树",
                tooltip: () => "显示果树",
                getValue: () => this.Config.ShowFruitTree,
                setValue: value => this.Config.ShowFruitTree = value
            );
        }

        private List<TerrainFeature>? GetLocationTerrainFeature(string locationName)
        {
            if (Context.IsWorldReady)
            {
                return GetLocationTerrainFeature(Game1.getLocationFromName(locationName));
            }
            return null;
        }

        private List<TerrainFeature>? GetLocationTerrainFeature(GameLocation location)
        {
            if (Context.IsWorldReady)
            {
                if (location != null)
                {
                    List<TerrainFeature> terrainFeatureList = new List<TerrainFeature>();
                    NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>> temp = location.terrainFeatures;
                    foreach (var terrainFeature in temp)
                    {
                        foreach (var terrain in terrainFeature)
                        {
                            terrainFeatureList.Add(terrain.Value);
                        }
                    }
                    return terrainFeatureList;
                }
            }
            return null;
        }

        private List<Building>? GetLocationBuildingList(string locationName)
        {
            if (Context.IsWorldReady)
            {
                return GetLocationBuildingList(Game1.getLocationFromName(locationName));
            }
            return null;
        }
        private List<Building>? GetLocationBuildingList(GameLocation location)
        {
            if (Context.IsWorldReady)
            {
                if (location != null)
                {
                    List<Building> buildingList = new List<Building>();
                    NetCollection<Building> temp = location.buildings;
                    foreach (var building in temp)
                    {
                        buildingList.Add(building);
                    }
                    return buildingList;
                }
            }
            return null;
        }

        private List<Object>? GetLocationObjectList(string locationName)
        {
            if (Context.IsWorldReady)
            {
                return GetLocationObjectList(Game1.getLocationFromName(locationName));
            }
            return null;
        }
        private List<Object>? GetLocationObjectList(GameLocation location)
        {
            if (Context.IsWorldReady)
            {
                if (location != null)
                {
                    List<Object> objectList = new List<Object>();
                    OverlaidDictionary temp = location.objects;
                    foreach (var obj in temp.Values)
                    {
                        objectList.Add(obj);
                    }
                    return objectList;
                }
            }
            return null;
        }
        //private void OnDisplayMenuChanged(object? sender, MenuChangedEventArgs e)
        //{
        //    if (e.NewMenu != null)
        //    {
        //        MyLog.Log($"{Game1.player.Name} open menu at {e.NewMenu.Position}.", LogLevel.Debug);
        //        return;
        //    }
        //}

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if (Config.ToggleFarmComputer.JustPressed())
            {
                ToggleMyMenu();
            }
        }

        private List<List<string>> GetFarmAnalyseStringLists()
        {
            var strs = new List<List<string>>();

            if (Config != null && !Config.ShowFarm)
                goto GreenHouse;
            //Farm
            Analyse.AnalyseTerrainFeatureList(GetLocationTerrainFeature("Farm"), out int hoeDirtCount_Farm, out int cropCount_Farm, out int readyForHarvestCount_Farm, out int needsWateringCount_Farm, out var _, out var _);
            Analyse.AnalyseBuildingList(GetLocationBuildingList("Farm"), out var buildingStruct_Farm);
            Analyse.GetPiecesOfHay(out int hayCount);
            Analyse.AnalyseObjectsList(GetLocationObjectList("Farm"), out AnalyseObjectStruct objStruct_Farm);
            var farmlist = new List<string>
            {
                $"{Game1.player.Name}的农场报告:",
                $"--------------",
                $"干草:{hayCount}/{buildingStruct_Farm.GetType(BuildingStructType.Hay).capacity}",
                $"农作物总量:{cropCount_Farm}",
                $"可收成农作物:{readyForHarvestCount_Farm}",
                $"未浇水农作物:{needsWateringCount_Farm}",
                $"开耕土壤:{hoeDirtCount_Farm - cropCount_Farm}",
                $"松露数量:{objStruct_Farm.GetType(ObjectStructType.Truffle).count}",
            };
            strs.Add(farmlist);

        GreenHouse:
            if (Config != null && !Config.ShowGreenHouse)
                goto IslandWest;
            //Greenhouse
            Analyse.AnalyseTerrainFeatureList(GetLocationTerrainFeature("GreenHouse"), out int hoeDirtCount_GreenHouse, out int cropCount_GreenHouse, out int readyForHarvestCount_GreenHouse, out int needsWateringCount_GreenHouse, out var _, out var _);
            var greenhouseList = new List<string>
            {
                $"{Game1.player.Name}的温室报告:",
                $"--------------",
                $"农作物总量:{cropCount_GreenHouse}",
                $"可收成农作物:{readyForHarvestCount_GreenHouse}",
                $"未浇水农作物:{needsWateringCount_GreenHouse}",
                $"开耕土壤:{hoeDirtCount_GreenHouse - cropCount_GreenHouse}"
            };
            strs.Add(greenhouseList);

        IslandWest:
            if (Config != null && !Config.ShowIslandWest)
                goto Other;
            //IslandWest
            Analyse.AnalyseTerrainFeatureList(GetLocationTerrainFeature("IslandWest"), out int hoeDirtCount_IslandWest, out int cropCount_IslandWest, out int readyForHarvestCount_IslandWest, out int needsWateringCount_IslandWest, out var _, out var _);
            var islandWestList = new List<string>
            {
                $"{Game1.player.Name}的姜岛报告:",
                $"--------------",
                $"农作物总量:{cropCount_IslandWest}",
                $"可收成农作物:{readyForHarvestCount_IslandWest}",
                $"未浇水农作物:{needsWateringCount_IslandWest}",
                $"开耕土壤:{hoeDirtCount_IslandWest - cropCount_IslandWest}"
            };
            strs.Add(islandWestList);

        Other:
            if (Config != null && !Config.ShowOther)
                goto End;
            //ALL
            IList<GameLocation> locations = Game1.locations;
            AnalyseBuildingStruct buildingStruct_ALL = new AnalyseBuildingStruct();
            AnalyseObjectStruct objsStruct_ALL = new AnalyseObjectStruct();

            Analyse.AnalyseObjectsList(GetLocationObjectList("Cellar"), out var objs_Cellar);

            int canHarvestFruitTree = 0;
            int fruitTreeCount = 0;

            foreach (GameLocation location in locations)
            {
                if (location != null)
                {
                    Analyse.AnalyseObjectsList(GetLocationObjectList(location), out var tempobj);
                    Analyse.AnalyseBuildingList(GetLocationBuildingList(location), out var tempbuilds);
                    Analyse.AnalyseTerrainFeatureList(GetLocationTerrainFeature(location), out var _, out var _, out var _, out var _, out var _canHarvestFruitTree, out var _fruitTreeCount);
                    objsStruct_ALL.Add(tempobj);
                    buildingStruct_ALL.Add(tempbuilds);
                    canHarvestFruitTree += _canHarvestFruitTree;
                    fruitTreeCount += _fruitTreeCount;
                }
            }


            var allList = new List<string>
            {
                $"{Game1.player.Name}的其它报告:",
                $"--------------",
            };
            if (Config == null || Config.ShowTapper)
            {
                var heaveTapper = objsStruct_ALL.GetType(ObjectStructType.HeavyTapper);
                allList.Add($"重型树液采集器:{heaveTapper.count}");
                allList.Add($"可收成重型树液采集器:{heaveTapper.readyForHarvestCount}");
                var tapper = objsStruct_ALL.GetType(ObjectStructType.Tapper);
                allList.Add($"树液采集器数量:{tapper.count}");
                allList.Add($"可收成树液采集器:{tapper.readyForHarvestCount}");
            }
            if (Config == null || Config.ShowKeg)
            {
                var keg = objsStruct_ALL.GetType(ObjectStructType.Keg) +
                    buildingStruct_ALL.analyseObjectStruct.GetType(ObjectStructType.Keg);
                allList.Add($"小桶数量:{keg.count}");
                allList.Add($"可收获小桶:{keg.readyForHarvestCount}");
                allList.Add($"空闲小桶:{keg.emptyCount}");
            }
            if (Config == null || Config.ShowBeeHouse)
            {
                var beehouse = objsStruct_ALL.GetType(ObjectStructType.BeeHouse) +
                    buildingStruct_ALL.analyseObjectStruct.GetType(ObjectStructType.BeeHouse);
                allList.Add($"蜂房数量:{beehouse.count}");
                allList.Add($"可收获蜂房:{beehouse.readyForHarvestCount}");
            }
            if (Config == null || Config.ShowCask)
            {
                var cask = objs_Cellar.GetType(ObjectStructType.Cask);
                allList.Add($"地窖木桶:{cask.count}");
                allList.Add($"可收获地窖木桶:{cask.readyForHarvestCount}");
                allList.Add($"空闲地窖木桶:{cask.emptyCount}");
            }
            if (Config == null || Config.ShowPreserveJar)
            {
                var preserveJar = objsStruct_ALL.GetType(ObjectStructType.PreserveJar) +
                    buildingStruct_ALL.analyseObjectStruct.GetType(ObjectStructType.PreserveJar);
                allList.Add($"罐头瓶:{preserveJar.count}");
                allList.Add($"可收获罐头瓶:{preserveJar.readyForHarvestCount}");
                allList.Add($"空闲罐头瓶:{preserveJar.emptyCount}");
            }
            if (Config == null || Config.ShowMushroomLog)
            {
                var mushroomLog = objsStruct_ALL.GetType(ObjectStructType.MushroomLog);
                allList.Add($"蘑菇树桩:{mushroomLog.count}");
                allList.Add($"可收获蘑菇树桩:{mushroomLog.readyForHarvestCount}");
            }
            if (Config == null || Config.ShowDehydrator)
            {
                var dehydrator = objsStruct_ALL.GetType(ObjectStructType.Dehydrator) +
                    buildingStruct_ALL.analyseObjectStruct.GetType(ObjectStructType.Dehydrator);
                allList.Add($"烘干机:{dehydrator.count}");
                allList.Add($"可收获烘干机:{dehydrator.readyForHarvestCount}");
                allList.Add($"空闲烘干机:{dehydrator.emptyCount}");
            }
            if (Config == null || Config.ShowFruitTree)
            {
                allList.Add($"果树:{fruitTreeCount}");
                allList.Add($"可收获果树:{canHarvestFruitTree}");
            }
            strs.Add(allList);

        End:
            //MyLog.Log($"{Game1.player.Name} {hoeDirtCount_Farm} {cropCount_Farm} {readyForHarvestCount_Farm} {needsWateringCount_Farm}", LogLevel.Debug);
            //MyLog.Log($"{str.Count}", LogLevel.Debug);
            return strs;
        }

        private void ToggleMyMenu()
        {
            currentMenuIdx = 0;
            if (Game1.activeClickableMenu is MyMenu)
                this.HideMyMenu();
            else
                this.ShowMyMenu(CurrentMenuIdx);
        }

        private void ShowMyMenu(int idx)
        {
            MyMenu myMenu = new MyMenu(this);
            myMenu.SetContentLists(GetAnalyseStringLists(idx));
            Game1.activeClickableMenu = myMenu;
            //myMenu.draw(null);
        }

        private List<List<string>> GetAnalyseStringLists(int currentMenuIdx)
        {
            switch (currentMenuIdx)
            {
                case 0:
                    return GetFarmAnalyseStringLists();
                case 1:
                    return GetStateStringLists();
                default:
                    return GetFarmAnalyseStringLists();
            }
        }

        private List<List<string>> GetStateStringLists()
        {
            List<List<string>> res = new List<List<string>>();

            Farmer key = Game1.player;
            float percentGameComplete = StardewValley.Utility.percentGameComplete();
            float getFarmerItemsShippedPercent = Utility.GetFarmCompletion((Farmer farmer) => Utility.getFarmerItemsShippedPercent(farmer)).Value;
            float getObeliskTypesBuilt = Math.Min(Utility.GetObeliskTypesBuilt(), 4f);
            bool isBuildingConstructed = Game1.IsBuildingConstructed("Gold Clock");
            bool hasCompletedAllMonsterSlayerQuests = Utility.GetFarmCompletion((Farmer farmer) => farmer.hasCompletedAllMonsterSlayerQuests.Value).Value;
            float getMaxedFriendshipPercent = Utility.GetFarmCompletion((Farmer farmer) => Utility.getMaxedFriendshipPercent(farmer)).Value;
            float farmLevel = Utility.GetFarmCompletion((Farmer farmer) => Math.Min(farmer.Level, 25f) / 25f).Value;
            bool foundAllStardrops = Utility.GetFarmCompletion((Farmer farmer) => Utility.foundAllStardrops(farmer)).Value;
            float getCookedRecipesPercent = Utility.GetFarmCompletion((Farmer farmer) => Utility.getCookedRecipesPercent(farmer)).Value;
            float getCraftedRecipesPercent = Utility.GetFarmCompletion((Farmer farmer) => Utility.getCraftedRecipesPercent(farmer)).Value;
            float getFishCaughtPercent = Utility.GetFarmCompletion((Farmer farmer) => Utility.getFishCaughtPercent(farmer)).Value;
            float GoldenWalnutsCount = 130f;
            float GoldenWalnutsFound = Math.Min(Game1.netWorldState.Value.GoldenWalnutsFound, GoldenWalnutsCount);

            string isBuildingConstructedStr = isBuildingConstructed ? "是" : "否";
            string foundAllStardropsStr = foundAllStardrops ? "是" : "否";
            string hasCompletedAllMonsterSlayerQuestsStr = hasCompletedAllMonsterSlayerQuests ? "是" : "否";
            var perfectList = new List<string>()
            {
                $"完美度统计",
                $"--------------",
                $"已售出的产品和采集品:{(getFarmerItemsShippedPercent * 100f).ToString("F2")}%",
                $"农场上的图腾柱:{getObeliskTypesBuilt}/4",
                $"农场上有黄金时钟:{isBuildingConstructedStr}",
                $"杀怪英雄:{hasCompletedAllMonsterSlayerQuestsStr}",
                $"好朋友:{(getMaxedFriendshipPercent * 100f).ToString("F2")}%",
                $"农场主等级:{farmLevel*25}/25",
                $"找到所有星之果实:{foundAllStardropsStr}",
                $"制作的烹饪食谱:{(getCookedRecipesPercent * 100f).ToString("F2")}%",
                $"制作的制造设计图:{(getCraftedRecipesPercent * 100f).ToString("F2")}%",
                $"捕获的鱼:{(getFishCaughtPercent * 100f).ToString("F2")}%",
                $"找到的金色核桃:{GoldenWalnutsFound}/{GoldenWalnutsCount}",
                $"--------------",
                $"总完成度:{(percentGameComplete * 100f).ToString("F2")}%",
            };
            res.Add(perfectList);

            //赌场信息
            var stats = Game1.player.stats;
            List<string> statList = new List<string>
            {
                "文件:" + Game1.player.Name,
                "迈出的步数:" + stats.StepsTaken,
                "送出的礼物:" + stats.GiftsGiven,
                "在星露谷住的天数:" + stats.DaysPlayed,
                "锄过的地:" + stats.DirtHoed,
                "制作的物品:" + stats.ItemsCrafted,
                "烹饪的菜式:" + stats.ItemsCooked,
                "回收的垃圾:" + stats.PiecesOfTrashRecycled,
                "杀死的怪物:" + stats.MonstersKilled,
                "抓捕的鱼类:" + stats.FishCaught,
                "投掷的渔线:" + stats.TimesFished,
                "播下的种子:" + stats.SeedsSown,
                "运送的物品:" + stats.ItemsShipped
            };
            res.Add(statList);
            return res;
        }

        private void HideMyMenu()
        {
            if (Game1.activeClickableMenu is MyMenu myMenu)
                myMenu.exitThisMenu();
        }
    }
}