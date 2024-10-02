using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterFarmComputer.Struct
{
    public enum BuildingStructType
    {
        Hay
    }
    public class BuildingStruct
    {
        public BuildingStructType type;
        public int capacity;
        public BuildingStruct(BuildingStructType type)
        {
            this.type = type;
        }

        public static BuildingStruct operator +(BuildingStruct p1, BuildingStruct p2)
        {
            BuildingStruct res = new BuildingStruct(BuildingStructType.Hay);
            res.capacity = p1.capacity + p2.capacity;
            return res;
        }
    }
}
