using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TritonSimulator.InternalModels
{
    public class Fleet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Star OriginStar { get; set; }
        public Star DestinationStar { get; set; }
        public Star CurrentStar { get; set; }
        public bool InTransit { get; set; }
        public int DistanceToDestination { get; set; }
        public int Ships { get; set; }
        public bool ToProcess { get; set; }
        public Player Owner { get; set; }
    }
}
