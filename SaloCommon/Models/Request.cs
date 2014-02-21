using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaloCommon.Models
{
    public class Request
    {
        public class OrderType
        {
            public const string FullUniverseReport = "full_universe_report";
        }

        public string type { get; set; }
        public string order { get; set; }
        public int version { get; set; }
        public string game_number { get; set; }
    }
}
