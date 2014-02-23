using System;

namespace TritonSimulator.InternalModels.Map
{
    public class MapStar
    {
        public int id { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        public bool isStartingStar { get; set; }
        public bool isHomeStar { get; set; }
        public int player { get; set; }
        public int resources { get; set; }
    }
}
