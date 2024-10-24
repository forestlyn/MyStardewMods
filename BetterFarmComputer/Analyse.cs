using BetterFarmComputer.Struct;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace BetterFarmComputer
{
    public class Analyse
    {
        public ObjectStructType GetObjectStructType(string itemid, out bool useEmpty, out bool useReadyForHaverst)
        {
            switch (itemid)
            {
                case ItemId.TruffleItemID:
                    useEmpty = false;
                    useReadyForHaverst = false;
                    return ObjectStructType.Truffle;
                case ItemId.HeavyTapperItemID:
                    useEmpty = false;
                    useReadyForHaverst = true;
                    return ObjectStructType.HeavyTapper;
                case ItemId.TapperItemID:
                    useEmpty = false;
                    useReadyForHaverst = true;
                    return ObjectStructType.Tapper;
                case ItemId.KegItemID:
                    useEmpty = true;
                    useReadyForHaverst = true;
                    return ObjectStructType.Keg;
                case ItemId.BeeHouseItemID:
                    useEmpty = false;
                    useReadyForHaverst = true;
                    return ObjectStructType.BeeHouse;
                case ItemId.CaskItemID:
                    useEmpty = true;
                    useReadyForHaverst = true;
                    return ObjectStructType.Cask;
                case ItemId.PreserveJarItemID:
                    useEmpty = true;
                    useReadyForHaverst = true;
                    return ObjectStructType.PreserveJar;
                case ItemId.MushroomLogItemID:
                    useEmpty = false;
                    useReadyForHaverst = true;
                    return ObjectStructType.MushroomLog;
                case ItemId.DehydratorItemID:
                    useEmpty = true;
                    useReadyForHaverst = true;
                    return ObjectStructType.Dehydrator;
                case ItemId.FishSmokerItemID:
                    useEmpty = true;
                    useReadyForHaverst = true;
                    return ObjectStructType.FishSmoker;
            }
            useEmpty = false;
            useReadyForHaverst = false;
            return ObjectStructType.None;
        }


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

        private void AnalyseBuilding(Building building, out BuildingStruct buildingStruct, out AnalyseObjectStruct analyseObjectStruct)
        {
            buildingStruct = new BuildingStruct(BuildingStructType.Hay);
            buildingStruct.capacity = building.hayCapacity.Value;
            analyseObjectStruct = new AnalyseObjectStruct();
            //OverlaidDictionary? objs = building.GetIndoors()?.objects;
            OverlaidDictionary? objs = building.indoors.Value?.objects;
            if (objs != null)
            {
                List<Object> objslist = new List<Object>();
                foreach (var obj in objs.Values)
                {
                    objslist.Add(obj);
                }
                AnalyseObjectsList(objslist, out var analyseObject);
                analyseObjectStruct = analyseObject;
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
                AnalyseBuilding(building, out var buildingStruct, out var analyseObjectStruct);
                analyseBuildingStruct.Add(buildingStruct.type, buildingStruct);
                analyseBuildingStruct.analyseObjectStruct.Add(analyseObjectStruct);
            }
        }


        private void AnalyseObjects(Object obj, out ObjectStruct objectStruct)
        {
            objectStruct = new ObjectStruct();
            if (obj == null || obj.itemId == null)
            {
                return;
            }

            objectStruct.count = 1;
            objectStruct.type = GetObjectStructType(obj.itemId.Value, out var useEmpty, out var useReadyForHaverst);
            objectStruct.useReadyForHarvestCount = useReadyForHaverst;
            objectStruct.useEmpty = useEmpty;
            if (useReadyForHaverst)
            {
                objectStruct.readyForHarvestCount = obj.readyForHarvest.Value ? 1 : 0;
            }
            if (useEmpty)
            {
                objectStruct.emptyCount = obj.MinutesUntilReady == 0 && obj.readyForHarvest.Value != true ? 1 : 0;
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
                AnalyseObjects(obj, out ObjectStruct objectStruct);
                if (objectStruct.type != ObjectStructType.None)
                    analyseObjectStruct.Add(objectStruct.type, objectStruct);
            }
        }

        private void AnalyseTerrainFeature(TerrainFeature terrainFeature, out bool isHoeDirt, out bool hasCrop, 
            out bool readyForHarvest, out bool needsWatering,out int fruitCount,out int fruitTreeCount)
        {
            isHoeDirt = false;
            readyForHarvest = false;
            hasCrop = false;
            needsWatering = false;
            fruitCount = 0;
            fruitTreeCount = 0;
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
            if(terrainFeature is FruitTree)
            {
                FruitTree tree = (FruitTree)terrainFeature;
                fruitCount = tree.fruit.Count;
                fruitTreeCount = 1;
            }
        }
        public void AnalyseTerrainFeatureList(List<TerrainFeature>? terrainFeatureList, out int hoeDirtCount, 
            out int cropCount, out int readyForHarvestCount, out int needsWateringCount,
            out int canHarvestFruitTree,out int fruitTreeCount)
        {
            hoeDirtCount = 0;
            cropCount = 0;
            readyForHarvestCount = 0;
            needsWateringCount = 0;
            canHarvestFruitTree = 0;
            fruitTreeCount = 0;
            if (terrainFeatureList == null)
            {
                return;
            }
            foreach (var terrain in terrainFeatureList)
            {
                AnalyseTerrainFeature(terrain, out bool isHoeDirt, out bool hasCrop, 
                    out bool readyForHarvest, out bool needsWatering, 
                    out int fruitCount, out int _fruitTreeCount);
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
                if(fruitCount > 0)
                {
                    canHarvestFruitTree++;
                }
                fruitTreeCount += _fruitTreeCount;
            }
        }

    }
}
