using StardewModdingAPI.Utilities;
using StardewModdingAPI;
using System.Numerics;

namespace ShowRealTime
{
    public class ModConfig
    {
        public bool use_24_hour_Clock { get; set; } = true;
        //public bool showDate { get; set; } = true;

        public int ClockNum { get; set; } = 3;

        public List<Clock> Clocks { get; set; } = new List<Clock>();

        public bool SetUIPosition { get; set; } = false;
        public int PositionX { get; set; } = 0;
        public int PositionY { get; set; } = 0;
    }

    public class Clock
    {
        public bool UseClock { get; set; } = false;

        private int hour;
        private int minute;

        public int Hour
        {
            get => hour;
            set
            {
                if (value < 0)
                {
                    hour = 0;
                }
                else if (value > 23)
                {
                    hour = 23;
                }
                else
                {
                    hour = value;
                }
            }
        }

        public int Minute
        {
            get => minute;
            set
            {
                if (value < 0)
                {
                    minute = 0;
                }
                else if (value > 59)
                {
                    minute = 59;
                }
                else
                {
                    minute = value;
                }
            }
        }
    }
}
