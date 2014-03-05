using Newtonsoft.Json;
using System.ComponentModel;

namespace Salo
{
    public class Star
    {
        public Star(Star star)
        {
            this.Economy = star.Economy;
            this.Id = star.Id;
            this.Industry = star.Industry;
            this.IsHomeStar = star.IsHomeStar;
            this.IsStartingStar = star.IsStartingStar;
            this.IsVisible = star.IsVisible;
            this.Name = star.Name;
            this.NaturalResources = star.NaturalResources;
            this.PlayerId = star.PlayerId;
            this.Science = star.Science;
            this.Ships = star.Ships;
            this.ShipsFractional = star.ShipsFractional;
            this.TotalResources = star.TotalResources;
            this.WarpGate = star.WarpGate;
            this.X = star.X;
            this.Y = star.Y;
            this.c = star.c;
        }

        public Star()
        {
            
        }

        public static class Upgrade
        {
            public const string WarpGate = "warpgate";
            public const string Economy = "economy";
            public const string Industry = "industry";
            public const string Science = "science";
        }

        /// <summary>
        /// Unique Star ID
        /// </summary>
        [JsonProperty("uid")]
        public int Id { get; set; }

        /// <summary>
        /// Name of the star
        /// </summary>
        [JsonProperty("n")]
        public string Name { get; set; }

        /// <summary>
        /// Player Unique ID of the owner of the star
        /// </summary>
        [JsonProperty("puid")]
        public int PlayerId { get; set; }

        /// <summary>
        /// Is the star Visible
        /// </summary>
        [JsonProperty("v")]
        public bool IsVisible { get; set; }

        /// <summary>
        /// Star's Y Position
        /// </summary>
        [JsonProperty("y")]
        public double Y { get; set; }

        /// <summary>
        /// Star's X Position
        /// </summary>
        [JsonProperty("x")]
        public double X { get; set; }

        /// <summary>
        /// Unsure
        /// </summary>
        [Browsable(false)]
        public int c { get; set; }

        /// <summary>
        /// Economy
        /// </summary>
        [JsonProperty("e")]
        public int Economy { get; set; }

        /// <summary>
        /// Industry
        /// </summary>
        [JsonProperty("i")]
        public int Industry { get; set; }

        /// <summary>
        /// Science
        /// </summary>
        [JsonProperty("s")]
        public int Science { get; set; }

        /// <summary>
        /// Total Resources
        /// </summary>
        [JsonProperty("r")]
        public int TotalResources { get; set; }

        /// <summary>
        /// Warp Gate
        /// </summary>
        [JsonProperty("ga")]
        public int WarpGate { get; set; }

        /// <summary>
        /// Natural Resources
        /// </summary>
        [JsonProperty("nr")]
        public int NaturalResources { get; set; }

        /// <summary>
        /// Number of ships on the star
        /// </summary>
        [JsonProperty("st")]
        public int Ships { get; set; }
        
        /*
         * Simulator Use Only
         */

        internal double ShipsFractional { get; set; }
        internal bool IsHomeStar { get; set; }
        internal bool IsStartingStar { get; set; }
    }
}
