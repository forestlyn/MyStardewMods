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
            MyHelper.SetHelper(helper);
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
                name: () => MyHelper.GetTranslation("openFarmInfoButton"),
                tooltip: () => MyHelper.GetTranslation("openFarmInfoButton")
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => MyHelper.GetTranslation("showFarmInfo"),
                tooltip: () => MyHelper.GetTranslation("showFarmInfo"),
                getValue: () => this.Config.ShowFarm,
                setValue: value => this.Config.ShowFarm = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => MyHelper.GetTranslation("showFarmInfo"),
                tooltip: () => MyHelper.GetTranslation("showGreenHouseInfo"),
                getValue: () => this.Config.ShowGreenHouse,
                setValue: value => this.Config.ShowGreenHouse = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => MyHelper.GetTranslation("showGingerIslandInfo"),
                tooltip: () => MyHelper.GetTranslation("showGingerIslandInfo"),
                getValue: () => this.Config.ShowIslandWest,
                setValue: value => this.Config.ShowIslandWest = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => MyHelper.GetTranslation("showOtherInfo"),
                tooltip: () => MyHelper.GetTranslation("showOtherInfo"),
                getValue: () => this.Config.ShowOther,
                setValue: value => this.Config.ShowOther = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => MyHelper.GetTranslation("showTapperInfo"),
                tooltip: () => MyHelper.GetTranslation("showTapperInfo"),
                getValue: () => this.Config.ShowTapper,
                setValue: value => this.Config.ShowTapper = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => MyHelper.GetTranslation("showKegInfo"),
                tooltip: () => MyHelper.GetTranslation("showKegInfo"),
                getValue: () => this.Config.ShowKeg,
                setValue: value => this.Config.ShowKeg = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => MyHelper.GetTranslation("showBeeHouseInfo"),
                tooltip: () => MyHelper.GetTranslation("showBeeHouseInfo"),
                getValue: () => this.Config.ShowBeeHouse,
                setValue: value => this.Config.ShowBeeHouse = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => MyHelper.GetTranslation("showCaskInfo"),
                tooltip: () => MyHelper.GetTranslation("showCaskInfo"),
                getValue: () => this.Config.ShowCask,
                setValue: value => this.Config.ShowCask = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => MyHelper.GetTranslation("showPreserveJarInfo"),
                tooltip: () => MyHelper.GetTranslation("showPreserveJarInfo"),
                getValue: () => this.Config.ShowPreserveJar,
                setValue: value => this.Config.ShowPreserveJar = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => MyHelper.GetTranslation("showMushroomLogInfo"),
                tooltip: () => MyHelper.GetTranslation("showMushroomLogInfo"),
                getValue: () => this.Config.ShowMushroomLog,
                setValue: value => this.Config.ShowMushroomLog = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => MyHelper.GetTranslation("showDehydratorInfo"),
                tooltip: () => MyHelper.GetTranslation("showDehydratorInfo"),
                getValue: () => this.Config.ShowDehydrator,
                setValue: value => this.Config.ShowDehydrator = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => MyHelper.GetTranslation("showFruitTreeInfo"),
                tooltip: () => MyHelper.GetTranslation("showFruitTreeInfo"),
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
                $"{Game1.player.Name} {MyHelper.GetTranslation("farmReport")}:",
                $"--------------",
                $"{MyHelper.GetTranslation("hay")}:{hayCount}/{buildingStruct_Farm.GetType(BuildingStructType.Hay).capacity}",
                $"{MyHelper.GetTranslation("cropCount")}:{cropCount_Farm}",
                $"{MyHelper.GetTranslation("readyForHarvestCount")}:{readyForHarvestCount_Farm}",
                $"{MyHelper.GetTranslation("needsWateringCount")}:{needsWateringCount_Farm}",
                $"{MyHelper.GetTranslation("hoeDirtCount")}:{hoeDirtCount_Farm - cropCount_Farm}",
                $"{MyHelper.GetTranslation("truffleCount")}:{objStruct_Farm.GetType(ObjectStructType.Truffle).count}",
            };
            strs.Add(farmlist);

        GreenHouse:
            if (Config != null && !Config.ShowGreenHouse)
                goto IslandWest;
            //Greenhouse
            Analyse.AnalyseTerrainFeatureList(GetLocationTerrainFeature("GreenHouse"), out int hoeDirtCount_GreenHouse, out int cropCount_GreenHouse, out int readyForHarvestCount_GreenHouse, out int needsWateringCount_GreenHouse, out var _, out var _);
            var greenhouseList = new List<string>
            {
                $"{Game1.player.Name} {MyHelper.GetTranslation("greenHouseReport")}:",
                $"--------------",
                $"{MyHelper.GetTranslation("cropCount")}:{cropCount_GreenHouse}",
                $"{MyHelper.GetTranslation("readyForHarvestCount")}:{readyForHarvestCount_GreenHouse}",
                $"{MyHelper.GetTranslation("needsWateringCount")}:{needsWateringCount_GreenHouse}",
                $"{MyHelper.GetTranslation("hoeDirtCount")}:{hoeDirtCount_GreenHouse - cropCount_GreenHouse}"
            };
            strs.Add(greenhouseList);

        IslandWest:
            if (Config != null && !Config.ShowIslandWest)
                goto Other;
            //IslandWest
            Analyse.AnalyseTerrainFeatureList(GetLocationTerrainFeature("IslandWest"), out int hoeDirtCount_IslandWest, out int cropCount_IslandWest, out int readyForHarvestCount_IslandWest, out int needsWateringCount_IslandWest, out var _, out var _);
            var islandWestList = new List<string>
            {
                $"{Game1.player.Name} {MyHelper.GetTranslation("gingerIslandReport")}:",
                $"--------------",
                $"{MyHelper.GetTranslation("cropCount")}:{cropCount_IslandWest}",
                $"{MyHelper.GetTranslation("readyForHarvestCount")}:{readyForHarvestCount_IslandWest}",
                $"{MyHelper.GetTranslation("needsWateringCount")}:{needsWateringCount_IslandWest}",
                $"{MyHelper.GetTranslation("hoeDirtCount")}:{hoeDirtCount_IslandWest - cropCount_IslandWest}"
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
                $"{Game1.player.Name} {MyHelper.GetTranslation("otherReport")}:",
                $"--------------",
            };
            if (Config == null || Config.ShowTapper)
            {
                var heaveTapper = objsStruct_ALL.GetType(ObjectStructType.HeavyTapper);
                allList.Add($"{MyHelper.GetTranslation("heaveTapper")}:{heaveTapper.count}");
                allList.Add($"{MyHelper.GetTranslation("heaveTapperReadyForHarvestCount")}:{heaveTapper.readyForHarvestCount}");
                var tapper = objsStruct_ALL.GetType(ObjectStructType.Tapper);
                allList.Add($"{MyHelper.GetTranslation("tapperCount")}:{tapper.count}");
                allList.Add($"{MyHelper.GetTranslation("tapperReadyForHarvestCount")}:{tapper.readyForHarvestCount}");
            }
            if (Config == null || Config.ShowKeg)
            {
                var keg = objsStruct_ALL.GetType(ObjectStructType.Keg) +
                    buildingStruct_ALL.analyseObjectStruct.GetType(ObjectStructType.Keg);
                allList.Add($"{MyHelper.GetTranslation("kegCount")}:{keg.count}");
                allList.Add($"{MyHelper.GetTranslation("kegReadyForHarvestCount")}:{keg.readyForHarvestCount}");
                allList.Add($"{MyHelper.GetTranslation("kegEmptyCount")}:{keg.emptyCount}");
            }
            if (Config == null || Config.ShowBeeHouse)
            {
                var beehouse = objsStruct_ALL.GetType(ObjectStructType.BeeHouse) +
                    buildingStruct_ALL.analyseObjectStruct.GetType(ObjectStructType.BeeHouse);
                allList.Add($"{MyHelper.GetTranslation("beehouseCount")}:{beehouse.count}");
                allList.Add($"{MyHelper.GetTranslation("beehouseReadyForHarvestCount")}:{beehouse.readyForHarvestCount}");
            }
            if (Config == null || Config.ShowCask)
            {
                var cask = objs_Cellar.GetType(ObjectStructType.Cask);
                allList.Add($"{MyHelper.GetTranslation("caskCount")}:{cask.count}");
                allList.Add($"{MyHelper.GetTranslation("caskReadyForHarvestCount")}:{cask.readyForHarvestCount}");
                allList.Add($"{MyHelper.GetTranslation("caskEmptyCount")}:{cask.emptyCount}");
            }
            if (Config == null || Config.ShowPreserveJar)
            {
                var preserveJar = objsStruct_ALL.GetType(ObjectStructType.PreserveJar) +
                    buildingStruct_ALL.analyseObjectStruct.GetType(ObjectStructType.PreserveJar);
                allList.Add($"{MyHelper.GetTranslation("preserveJarCount")}:{preserveJar.count}");
                allList.Add($"{MyHelper.GetTranslation("preserveJarReadyForHarvestCount")}:{preserveJar.readyForHarvestCount}");
                allList.Add($"{MyHelper.GetTranslation("preserveJarEmptyCount")}:{preserveJar.emptyCount}");
            }
            if (Config == null || Config.ShowMushroomLog)
            {
                var mushroomLog = objsStruct_ALL.GetType(ObjectStructType.MushroomLog);
                allList.Add($"{MyHelper.GetTranslation("mushroomLogCount")}:{mushroomLog.count}");
                allList.Add($"{MyHelper.GetTranslation("mushroomLogReadyForHarvestCount")}:{mushroomLog.readyForHarvestCount}");
            }
            if (Config == null || Config.ShowDehydrator)
            {
                var dehydrator = objsStruct_ALL.GetType(ObjectStructType.Dehydrator) +
                    buildingStruct_ALL.analyseObjectStruct.GetType(ObjectStructType.Dehydrator);
                allList.Add($"{MyHelper.GetTranslation("dehydratorCount")}:{dehydrator.count}");
                allList.Add($"{MyHelper.GetTranslation("dehydratorReadyForHarvestCount")}:{dehydrator.readyForHarvestCount}");
                allList.Add($"{MyHelper.GetTranslation("dehydratorEmptyCount")}:{dehydrator.emptyCount}");
            }
            if (Config == null || Config.ShowFruitTree)
            {
                allList.Add($"{MyHelper.GetTranslation("fruitTreeCount")}:{fruitTreeCount}");
                allList.Add($"{MyHelper.GetTranslation("canHarvestFruitTree")}:{canHarvestFruitTree}");
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

            string yesStr = MyHelper.GetTranslation("yes");
            string noStr = MyHelper.GetTranslation("no");
            string isBuildingConstructedStr = isBuildingConstructed ? yesStr : noStr;
            string foundAllStardropsStr = foundAllStardrops ? yesStr : noStr;
            string hasCompletedAllMonsterSlayerQuestsStr = hasCompletedAllMonsterSlayerQuests ? yesStr : noStr;
            var perfectList = new List<string>()
            {
                $"{MyHelper.GetTranslation("perfectionTracker")}",
                $"--------------",
                $"{MyHelper.GetTranslation("farmerItemsShipped")}:{(getFarmerItemsShippedPercent * 100f).ToString("F2")}%",
                $"{MyHelper.GetTranslation("obeliskTypesBuilt")}:{getObeliskTypesBuilt}/4",
                $"{MyHelper.GetTranslation("isBuildingConstructed")}:{isBuildingConstructedStr}",
                $"{MyHelper.GetTranslation("completedAllMonsterSlayerQuests")}:{hasCompletedAllMonsterSlayerQuestsStr}",
                $"{MyHelper.GetTranslation("bestFriends")}:{(getMaxedFriendshipPercent * 100f).ToString("F2")}%",
                $"{MyHelper.GetTranslation("farmLevel")}:{farmLevel*25}/25",
                $"{MyHelper.GetTranslation("foundAllStardrops")}:{foundAllStardropsStr}",
                $"{MyHelper.GetTranslation("cookedRecipes")}:{(getCookedRecipesPercent * 100f).ToString("F2")}%",
                $"{MyHelper.GetTranslation("craftedRecipes")}:{(getCraftedRecipesPercent * 100f).ToString("F2")}%",
                $"{MyHelper.GetTranslation("fishCaughtPercent")}:{(getFishCaughtPercent * 100f).ToString("F2")}%",
                $"{MyHelper.GetTranslation("goldenWalnutsFound")}:{GoldenWalnutsFound}/{GoldenWalnutsCount}",
                $"--------------",
                $"{MyHelper.GetTranslation("percentGameComplete")}:{(percentGameComplete * 100f).ToString("F2")}%",
            };
            res.Add(perfectList);

            //赌场信息
            var stats = Game1.player.stats;
            List<string> statList = new List<string>
            {
                $"{MyHelper.GetTranslation("file")}:" + Game1.player.Name,
                $"{MyHelper.GetTranslation("stepsTaken")}:" + stats.StepsTaken,
                $"{MyHelper.GetTranslation("GiftsGiven")}:" + stats.GiftsGiven,
                $"{MyHelper.GetTranslation("DaysPlayed")}:" + stats.DaysPlayed,
                $"{MyHelper.GetTranslation("DirtHoed")}:" + stats.DirtHoed,
                $"{MyHelper.GetTranslation("ItemsCrafted")}:" + stats.ItemsCrafted,
                $"{MyHelper.GetTranslation("ItemsCooked")}:" + stats.ItemsCooked,
                $"{MyHelper.GetTranslation("PiecesOfTrashRecycled")}:" + stats.PiecesOfTrashRecycled,
                $"{MyHelper.GetTranslation("MonstersKilled")}:" + stats.MonstersKilled,
                $"{MyHelper.GetTranslation("FishCaught")}:" + stats.FishCaught,
                $"{MyHelper.GetTranslation("TimesFished")}:" + stats.TimesFished,
                $"{MyHelper.GetTranslation("SeedsSown")}:" + stats.SeedsSown,
                $"{MyHelper.GetTranslation("ItemsShipped")}:" + stats.ItemsShipped
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