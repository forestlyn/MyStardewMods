using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace BetterFarmComputer
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {

        private Analyse analyse = new Analyse();
        public Analyse Analyse { get { return analyse; } }

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            //helper.Events.Display.MenuChanged += this.OnDisplayMenuChanged;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            MyLog.Monitor = this.Monitor;
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
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if (e.Button == SButton.L)
            {
                ToggleMyMenu();
            }
        }

        private List<List<string>> GetAnalyseStringLists()
        {
            var strs = new List<List<string>>();
            //Farm
            Analyse.AnalyseTerrainFeatureList(GetLocationTerrainFeature("Farm"), out int hoeDirtCount_Farm, out int cropCount_Farm, out int readyForHarvestCount_Farm, out int needsWateringCount_Farm);
            Analyse.AnalyseBuildingList(GetLocationBuildingList("Farm"), out var buildingStruct_Farm);
            Analyse.GetPiecesOfHay(out int hayCount);
            Analyse.AnalyseObjectsList(GetLocationObjectList("Farm"), out AnalyseObjectStruct objStruct_Farm);
            var farmlist = new List<string>
            {
                $"{Game1.player.Name}的农场报告:",
                $"--------------",
                $"干草:{hayCount}/{buildingStruct_Farm.hayCapacity}",
                $"农作物总量:{cropCount_Farm}",
                $"可收成农作物:{readyForHarvestCount_Farm}",
                $"未浇水农作物:{needsWateringCount_Farm}",
                $"开耕土壤:{hoeDirtCount_Farm - cropCount_Farm}",
                $"松露数量:{objStruct_Farm.truffleCount}",
            };
            strs.Add(farmlist);

            //Greenhouse
            Analyse.AnalyseTerrainFeatureList(GetLocationTerrainFeature("GreenHouse"), out int hoeDirtCount_GreenHouse, out int cropCount_GreenHouse, out int readyForHarvestCount_GreenHouse, out int needsWateringCount_GreenHouse);
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


            //IslandWest
            Analyse.AnalyseTerrainFeatureList(GetLocationTerrainFeature("IslandWest"), out int hoeDirtCount_IslandWest, out int cropCount_IslandWest, out int readyForHarvestCount_IslandWest, out int needsWateringCount_IslandWest);
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


            //ALL
            IList<GameLocation> locations = Game1.locations;
            AnalyseBuildingStruct buildingStruct_ALL = new AnalyseBuildingStruct();
            AnalyseObjectStruct objsStruct_ALL = new AnalyseObjectStruct();

            foreach (GameLocation location in locations)
            {
                if (location != null)
                {
                    Analyse.AnalyseObjectsList(GetLocationObjectList(location), out var tempobj);
                    Analyse.AnalyseBuildingList(GetLocationBuildingList(location), out var tempbuilds);
                    objsStruct_ALL += tempobj;
                    buildingStruct_ALL += tempbuilds;
                }
            }


            var allList = new List<string>
            {
                $"{Game1.player.Name}的其它报告:",
                $"--------------",
                $"重型树液采集器:{objsStruct_ALL.heavyTapperCount}",
                $"可收成重型树液采集器:{objsStruct_ALL.heavyTapperReadyForHarvestCount}",
                $"树液采集器数量:{objsStruct_ALL.tapperCount}",
                $"可收成树液采集器:{objsStruct_ALL.tapperReadyForHarvestCount}",
                $"小桶数量:{buildingStruct_ALL.kegCount+objsStruct_ALL.kegCount}",
                $"可收获小桶:{buildingStruct_ALL.kegReadyForHarvestCount+objsStruct_ALL.kegReadyForHarvestCount}",
                $"空闲小桶:{buildingStruct_ALL.kegIsEmpty + objsStruct_ALL.kegIsEmpty}",
                $"蜂房:{buildingStruct_ALL.beeHouseCount+objsStruct_ALL.beeHouseCount}",
                $"可收获蜂房:{buildingStruct_ALL.beeHouseReadyForHarvestCount + objsStruct_ALL.beeHouseReadyForHarvestCount}",
            };
            strs.Add(allList);


            //MyLog.Log($"{Game1.player.Name} {hoeDirtCount_Farm} {cropCount_Farm} {readyForHarvestCount_Farm} {needsWateringCount_Farm}", LogLevel.Debug);
            //MyLog.Log($"{str.Count}", LogLevel.Debug);
            return strs;
        }

        private void ToggleMyMenu()
        {
            if (Game1.activeClickableMenu is MyMenu)
                this.HideMyMenu();
            else
                this.ShowMyMenu();
        }

        private void ShowMyMenu()
        {
            MyMenu myMenu = new MyMenu();
            myMenu.SetContentLists(GetAnalyseStringLists());
            Game1.activeClickableMenu = myMenu;
            //myMenu.draw(null);
        }

        private void HideMyMenu()
        {
            if (Game1.activeClickableMenu is MyMenu myMenu)
                myMenu.exitThisMenu();
        }
    }
}