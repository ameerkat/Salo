using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Salo.Live.Models
{
    /// <summary>
    /// TODO Check this!
    /// </summary>
    public static class Research
    {
        public const string Weapons = "weapons";
        public const string Banking = "banking";
        public const string Propulsion = "propulsion";
        public const string Terraforming = "terraforming";
        public const string Experimentation = "research";
        public const string Scanning = "scanning";
        public const string Manufacturing = "manufacturing";
    }

    public class Player
    {
        [JsonProperty("total_industry")]
        public int TotalIndustry { get; set; }

        [JsonProperty("researching")]
        public string Researching { get; set; }

        [JsonProperty("total_science")]
        public int TotalScience { get; set; }

        /// <summary>
        /// Unique player ID
        /// </summary>
        [JsonProperty("Id")]
        public int Id { get; set; }

        /// <summary>
        /// Is the player an AI?
        /// </summary>
        [JsonProperty("ai")]
        public bool IsBot { get; set; }

        /// <summary>
        /// Unsure
        /// </summary>
        [Browsable(false)]
        public int huid { get; set; }

        /// <summary>
        /// Total number of owned stars in galaxy
        /// </summary>
        [JsonProperty("total_stars")]
        public int TotalStars { get; set; }

        [JsonProperty("cash")]
        public int Cash { get; set; }

        [JsonProperty("total_fleets")]
        public int TotalFleets { get; set; }

        /// <summary>
        /// Total number of ships
        /// </summary>
        [JsonProperty("total_strength")]
        public int TotalShips { get; set; }

        /// <summary>
        /// Displayed name of the player
        /// </summary>
        [JsonProperty("alias")]
        public string Name { get; set; }

        [JsonProperty("tech")]
        public Dictionary<string, Technology> Technology { get; set; }

        /// <summary>
        /// Avatar ID
        /// </summary>
        [JsonProperty("avatar")]
        public int AvatarId { get; set; }

        [JsonProperty("conceded")]
        public bool HasConceded { get; set; }

        [JsonProperty("research_next")]
        public string ResearchingNext { get; set; }

        /// <summary>
        /// Unsure where this gets used
        /// </summary>
        [JsonProperty("ready")]
        public bool IsReady { get; set; }

        [JsonProperty("total_economy")]
        public int TotalEconomy { get; set; }

        [Browsable(false)]
        public Dictionary<int, int> countdown_to_war { get; set; }

        [Browsable(false)]
        public Dictionary<int, int> war { get; set; }

        [Browsable(false)]
        public int missed_turns { get; set; }

        /// <summary>
        /// Renown
        /// </summary>
        [JsonProperty("karma_to_give")]
        public int Reknown { get; set; }
    }
}
