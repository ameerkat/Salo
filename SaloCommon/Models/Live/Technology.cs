using Newtonsoft.Json;
using System.ComponentModel;

namespace Salo.Live.Models
{
    public class Technology
    {
        /// <summary>
        /// Current tech level
        /// </summary>
        [JsonProperty("level")]
        public int Level { get; set; }

        /// <summary>
        /// Unsure
        /// </summary>
        [Browsable(false)]
        public double sv { get; set; }

        /// <summary>
        /// Unsure
        /// </summary>
        [Browsable(false)]
        public double value { get; set; }

        /// <summary>
        /// Current research added to this
        /// </summary>
        [JsonProperty("research")]
        public int Amount { get; set; }

        /// <summary>
        /// Unsure
        /// </summary>
        [Browsable(false)]
        public double bv { get; set; }

        /// <summary>
        /// All 144, perhaps base required research
        /// </summary>
        [JsonProperty("brr")]
        public int BaseRequiredResearch { get; set; }
    }
}
