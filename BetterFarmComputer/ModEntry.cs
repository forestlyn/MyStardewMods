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
                if (Game1.getLocationFromName(locationName) != null)
                {
                    List<TerrainFeature> terrainFeatureList = new List<TerrainFeature>();
                    NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>> temp = Game1.getLocationFromName(locationName).terrainFeatures;
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
                if (Game1.getLocationFromName(locationName) != null)
                {
                    List<Building> buildingList = new List<Building>();
                    NetCollection<Building> temp = Game1.getLocationFromName(locationName).buildings;
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
                if (Game1.getLocationFromName(locationName) != null)
                {
                    List<Object> objectList = new List<Object>();
                    OverlaidDictionary temp = Game1.getLocationFromName(locationName).objects;
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
            Analyse.AnalyseObjectsList(GetLocationObjectList("Farm"), out AnalyseObjectStruct truffleCount_Farm);
            var farmlist = new List<string>
            {
                $"{Game1.player.Name}的农场报告:",
                $"--------------",
                $"干草:{hayCount}/{buildingStruct_Farm.hayCapacity}",
                $"农作物总量:{cropCount_Farm}",
                $"可收成农作物:{readyForHarvestCount_Farm}",
                $"未浇水农作物:{needsWateringCount_Farm}",
                $"开耕土壤:{hoeDirtCount_Farm - cropCount_Farm}",
                $"松露数量:{truffleCount_Farm.truffleCount}",
                $"重型树液采集器:{truffleCount_Farm.heavyTapperCount}",
                $"可收成重型树液采集器:{truffleCount_Farm.heavyTapperReadyForHarvestCount}",
                $"树液采集器数量:{truffleCount_Farm.tapperCount}",
                $"可收成树液采集器:{truffleCount_Farm.tapperReadyForHarvestCount}",
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