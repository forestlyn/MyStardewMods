using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterFarmComputer
{
    internal struct AnalyseBuildingStruct
    {
        public AnalyseBuildingStruct()
        {
            hayCapacity = 0;
            kegCount = 0;
            kegIsEmpty = 0;
            kegReadyForHarvestCount = 0;
        }
        public int hayCapacity;
        public int kegCount;
        public int kegReadyForHarvestCount;
        public int kegIsEmpty;

        public static AnalyseBuildingStruct operator +(AnalyseBuildingStruct p1, AnalyseBuildingStruct p2)
        {
            AnalyseBuildingStruct res = new AnalyseBuildingStruct();
            res.hayCapacity = p1.hayCapacity + p2.hayCapacity;
            res.kegCount = p1.kegCount + p2.kegCount;
            res.kegReadyForHarvestCount = p1.kegReadyForHarvestCount + p2.kegReadyForHarvestCount;
            res.kegIsEmpty = p1.kegIsEmpty + p2.kegIsEmpty;
            return res;
        }

    }

    internal struct AnalyseObjectStruct
    {
        public AnalyseObjectStruct()
        {
            truffleCount = 0;
            heavyTapperCount = 0;
            heavyTapperReadyForHarvestCount = 0;
            tapperCount = 0;
            tapperReadyForHarvestCount = 0;
        }
        public int truffleCount;
        public int heavyTapperCount;
        public int heavyTapperReadyForHarvestCount;
        public int tapperCount;
        public int tapperReadyForHarvestCount;

        public static AnalyseObjectStruct operator +(AnalyseObjectStruct p1, AnalyseObjectStruct p2)
        {
            var res = new AnalyseObjectStruct();
            res.truffleCount = p1.truffleCount + p2.truffleCount;
            res.heavyTapperCount = p1.heavyTapperCount + p2.heavyTapperCount;
            res.heavyTapperReadyForHarvestCount = p1.heavyTapperReadyForHarvestCount + p2.heavyTapperReadyForHarvestCount;
            res.tapperCount = p1.tapperCount + p2.tapperCount;
            res.tapperReadyForHarvestCount = p1.tapperReadyForHarvestCount + p2.tapperReadyForHarvestCount;
            return res;
        }
    }
}
