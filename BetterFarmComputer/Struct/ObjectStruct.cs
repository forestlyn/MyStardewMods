using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterFarmComputer.Struct
{
    public enum ObjectStructType
    {
        None,
        Truffle,
        HeavyTapper,
        Tapper,
        Keg,
        BeeHouse,
        Cask,
        PreserveJar,
        MushroomLog,
        Dehydrator,
        FishSmoker
    }
    public class ObjectStruct
    {
        public ObjectStructType type;
        public int count;

        public bool useReadyForHarvestCount;
        public int readyForHarvestCount;

        public bool useEmpty;
        public int emptyCount;

        public ObjectStruct(ObjectStructType type)
        {
            this.type = type;
        }
        public ObjectStruct()
        {
            type = ObjectStructType.None;
        }

        public static ObjectStruct operator +(ObjectStruct p1, ObjectStruct p2)
        {
            if (p1.type != p2.type)
                throw new Exception("ObjectStructType not the same");
            ObjectStruct res = new ObjectStruct(p1.type);
            res.count = p1.count + p2.count;
            res.readyForHarvestCount = p1.readyForHarvestCount + p2.readyForHarvestCount;
            res.emptyCount = p1.emptyCount + p2.emptyCount;
            res.useEmpty = p1.useEmpty;
            res.useReadyForHarvestCount = p1.useReadyForHarvestCount;
            return res;
        }
    }

}
