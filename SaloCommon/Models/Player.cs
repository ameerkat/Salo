using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaloCommon.Models
{
    /// <summary>
    /// TODO Check this!
    /// </summary>
    public class Research
    {
        public const string weapons = "weapons";
        public const string banking = "banking";
        public const string propulsion = "propulsion";
        public const string terraforming = "terraforming";
        public const string research = "research";
        public const string scanning = "scanning";
        public const string manufacturing = "manufacturing";
    }

    public class Player
    {
        public int total_industry { get; set; }
        public string researching { get; set; }
        public int total_science { get; set; }

        /// <summary>
        /// Unique player ID
        /// </summary>
        public int uid { get; set; }

        /// <summary>
        /// Is the player an AI?
        /// </summary>
        public bool ai { get; set; }

        /// <summary>
        /// Unsure
        /// </summary>
        public int huid { get; set; }

        /// <summary>
        /// Total number of owned stars in galaxy
        /// </summary>
        public int total_stars { get; set; }

        public int cash { get; set; }

        public int total_fleets { get; set; }

        /// <summary>
        /// Total number of ships
        /// </summary>
        public int total_strength { get; set; }

        /// <summary>
        /// Displayed name of the player
        /// </summary>
        public string alias { get; set; }

        public Dictionary<string, Technology> tech { get; set; }

        /// <summary>
        /// Avatar ID
        /// </summary>
        public int avatar { get; set; }

        public bool conceded { get; set; }

        public string research_next { get; set; }

        /// <summary>
        /// Unsure where this gets used
        /// </summary>
        public bool ready { get; set; }

        public int total_economy { get; set; }

        public Dictionary<int, int> countdown_to_war { get; set; }

        public Dictionary<int, int> war { get; set; }

        public int missed_turns { get; set; }

        /// <summary>
        /// Renown
        /// </summary>
        public int karma_to_give { get; set; }
    }
}
