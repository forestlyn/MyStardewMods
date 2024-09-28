using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace BetterFarmComputer
{
    internal class Analyse
    {

        private const string TruffleItemID = "430";
        private const string HeavyTapperItemID = "264";
        private const string TapperItemID = "105";
        private const string KegItemID = "12";
        private const string BeeHouseItemID = "10";



        public void GetPiecesOfHay(out int hayCount)
        {
            hayCount = 0;
            if (Context.IsWorldReady)
            {
                if (Game1.getLocationFromName("Farm") != null)
                {
                    NetInt piecesOfHay = Game1.getLocationFromName("Farm").piecesOfHay;
                    hayCount = piecesOfHay.Value;
                }
            }
        }

        private void AnalyseBuilding(Building building, out AnalyseBuildingStruct analyseBuildingStruct)
        {
            analyseBuildingStruct = new AnalyseBuildingStruct();
            analyseBuildingStruct.hayCapacity = building.hayCapacity.Value;
            //OverlaidDictionary? objs = building.GetIndoors()?.objects;
            OverlaidDictionary? objs = building.indoors.Value?.objects;
            if (objs != null)
            {
                List<Object> objslist = new List<Object>();
                foreach(var obj in objs.Values)
                {
                    objslist.Add(obj);
                }
                AnalyseObjectsList(objslist, out var analyseObjectStruct);
                analyseBuildingStruct.kegCount = analyseObjectStruct.kegCount;
                analyseBuildingStruct.kegIsEmpty = analyseObjectStruct.kegIsEmpty;
                analyseBuildingStruct.kegReadyForHarvestCount = analyseObjectStruct.kegReadyForHarvestCount;
                analyseBuildingStruct.beeHouseCount = analyseObjectStruct.beeHouseCount;
                analyseBuildingStruct.beeHouseReadyForHarvestCount = analyseObjectStruct.beeHouseReadyForHarvestCount;
            }
        }

        public void AnalyseBuildingList(List<Building>? buildings, out AnalyseBuildingStruct analyseBuildingStruct)
        {
            analyseBuildingStruct = new AnalyseBuildingStruct();
            if (buildings is null)
            {
                return;
            }
            foreach (Building building in buildings)
            {
                AnalyseBuilding(building, out var buildingStruct);
                analyseBuildingStruct += buildingStruct;
            }
        }


        private void AnalyseObjects(Object obj, out AnalyseObjectStruct analyseObjectStruct)
        {
            analyseObjectStruct = new AnalyseObjectStruct();
            if (obj != null && obj.itemId != null && obj.itemId.Value == TruffleItemID)
            {
                analyseObjectStruct.truffleCount = 1;
            }
            else if (obj != null && obj.itemId != null && obj.itemId.Value == HeavyTapperItemID)
            {
                analyseObjectStruct.heavyTapperCount = 1;
                analyseObjectStruct.heavyTapperReadyForHarvestCount = obj.readyForHarvest.Value ? 1 : 0;
            }
            else if (obj != null && obj.itemId != null && obj.itemId.Value == TapperItemID)
            {
                analyseObjectStruct.tapperCount = 1;
                analyseObjectStruct.tapperReadyForHarvestCount = obj.readyForHarvest.Value ? 1 : 0;
            }
            else if (obj != null && obj.itemId != null && obj.itemId.Value == KegItemID)
            {
                analyseObjectStruct.kegCount = 1;
                analyseObjectStruct.kegReadyForHarvestCount = obj.readyForHarvest.Value ? 1 : 0;
                analyseObjectStruct.kegIsEmpty = (obj.minutesUntilReady.Value == 0 && obj.readyForHarvest.Value == false) ? 1 : 0;
            }
            else if (obj != null && obj.itemId != null && obj.itemId.Value == BeeHouseItemID)
            {
                analyseObjectStruct.beeHouseCount = 1;
                analyseObjectStruct.beeHouseReadyForHarvestCount = obj.readyForHarvest.Value ? 1 : 0;
            }
        }

        public void AnalyseObjectsList(List<Object>? objs, out AnalyseObjectStruct analyseObjectStruct)
        {
            analyseObjectStruct = new AnalyseObjectStruct();
            if (objs is null)
            {
                return;
            }
            foreach (Object obj in objs)
            {
                AnalyseObjects(obj, out AnalyseObjectStruct objectStruct);
                analyseObjectStruct += objectStruct;
            }
        }

        private void AnalyseTerrainFeature(TerrainFeature terrainFeature, out bool isHoeDirt, out bool hasCrop, out bool readyForHarvest, out bool needsWatering)
        {
            isHoeDirt = false;
            readyForHarvest = false;
            hasCrop = false;
            needsWatering = false;
            if (terrainFeature is null)
            {
                return;
            }
            if (terrainFeature is HoeDirt)
            {
                try
                {
                    HoeDirt hoeDirt = (HoeDirt)terrainFeature;
                    isHoeDirt = true;
                    if (hoeDirt.crop != null)
                    {
                        hasCrop = true;
                        readyForHarvest = hoeDirt.readyForHarvest();
                        if (hoeDirt.needsWatering())
                        {
                            needsWatering = !hoeDirt.isWatered();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MyLog.Log($"{ex}", LogLevel.Debug);
                }
            }
        }
        public void AnalyseTerrainFeatureList(List<TerrainFeature>? terrainFeatureList, out int hoeDirtCount, out int cropCount, out int readyForHarvestCount, out int needsWateringCount)
        {
            hoeDirtCount = 0;
            cropCount = 0;
            readyForHarvestCount = 0;
            needsWateringCount = 0;
            if (terrainFeatureList == null)
            {
                return;
            }
            foreach (var terrain in terrainFeatureList)
            {
                AnalyseTerrainFeature(terrain, out bool isHoeDirt, out bool hasCrop, out bool readyForHarvest, out bool needsWatering);
                if (isHoeDirt)
                {
                    hoeDirtCount++;
                }
                if (hasCrop)
                {
                    cropCount++;
                }
                if (readyForHarvest)
                {
                    readyForHarvestCount++;
                }
                if (needsWatering)
                {
                    needsWateringCount++;
                }
            }
        }

    }
}
