using StardewModdingAPI.Utilities;
using StardewModdingAPI;

namespace BetterFarmComputer
{
    public class ModConfig
    {
        public KeybindList ToggleFarmComputer { get; set; } = new(SButton.L);

        public bool ShowFarm { get; set; } = true;
        public bool ShowGreenHouse { get;  set; } = true;
        public bool ShowIslandWest { get;  set; } = true;
        public bool ShowOther { get; set; } = true;

        public bool ShowTapper { get; set; } = true;
        public bool ShowKeg { get; set; } = true;
        public bool ShowBeeHouse { get; set; } = true;
        public bool ShowCask { get; set; } = true;
        public bool ShowPreserveJar { get; set; } = true;
        public bool ShowMushroomLog { get; set; } = true;
        public bool ShowDehydrator { get; set; } = true;
        public bool ShowFruitTree { get; set; } = true;


    }
}
