using BetterFarmComputer.Struct;
using StardewModdingAPI;

namespace BetterFarmComputer
{
    public struct AnalyseBuildingStruct
    {
        Dictionary<BuildingStructType,BuildingStruct> buildingStructs;

        public AnalyseObjectStruct analyseObjectStruct;
        public AnalyseBuildingStruct()
        {
            analyseObjectStruct = new AnalyseObjectStruct();
            buildingStructs = new Dictionary<BuildingStructType, BuildingStruct>();
        }


        public void Add(BuildingStructType type, BuildingStruct buildingStruct)
        {
            if (!buildingStructs.ContainsKey(type))
                buildingStructs.Add(type, buildingStruct);
            else
            {
                buildingStructs[type] += buildingStruct;
            }
        }

        internal BuildingStruct GetType(BuildingStructType type)
        {
            if(!buildingStructs.ContainsKey(type))
                return new BuildingStruct(type); 
            return buildingStructs[type];
        }

        internal void Add(AnalyseBuildingStruct tempbuilds)
        {
            foreach(var build in tempbuilds.buildingStructs)
            {
                Add(build.Key, build.Value);
            }
            analyseObjectStruct.Add(tempbuilds.analyseObjectStruct);
        }
    }

    public struct AnalyseObjectStruct
    {
        public Dictionary<ObjectStructType, ObjectStruct> objectStructs;
        public AnalyseObjectStruct()
        {
            objectStructs = new Dictionary<ObjectStructType, ObjectStruct>();
        }


        public void Add(ObjectStructType type, ObjectStruct objectStruct)
        {
            if (!objectStructs.ContainsKey(type))
                objectStructs.Add(type, objectStruct);
            else
            {
                objectStructs[type] += objectStruct;
            }
        }

        public void Add(AnalyseObjectStruct analyseObjectStruct)
        {
            foreach (var obj in analyseObjectStruct.objectStructs)
            {
                Add(obj.Key, obj.Value);
            }
        }

        public ObjectStruct GetType(ObjectStructType type)
        {
            if (!objectStructs.ContainsKey(type))
            {
                //MyLog.Log(type.ToString() + " not found", LogLevel.Error);
                return new ObjectStruct(type);
            }
            return objectStructs[type];
        }
    }
}
