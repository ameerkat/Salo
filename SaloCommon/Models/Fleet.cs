using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TritonSimulator.Models
{
    public class Fleet
    {
        /// <summary>
        /// Optional, Unsure?
        /// </summary>
        public int ouid { get; set; }

        /// <summary>
        /// Unique ID for Fleet (used as dictionary index)
        /// </summary>
        public int uid { get; set; }

        /// <summary>
        /// Unsure, all Zeroes
        /// </summary>
        public int l { get; set; }

        /// <summary>
        /// Unsure, array of arrays only shows up for moving carriers
        /// Possible only for your own carriers, orders?
        /// </summary>
        public int[][] o { get; set; }

        /// <summary>
        /// Carrier Name
        /// </summary>
        public string n { get; set; }

        /// <summary>
        /// Player Unique Id
        /// </summary>
        public int puid { get; set; }

        /// <summary>
        /// Unsure, all Zeroes
        /// </summary>
        public int w { get; set; }

        /// <summary>
        /// Current y coordinate position
        /// </summary>
        public double y { get; set; }

        /// <summary>
        /// Current x coordinate position
        /// </summary>
        public double x { get; set; }

        /// <summary>
        /// Number of ships loaded
        /// </summary>
        public int st { get; set; }
        public double lx { get; set; }
        public double ly { get; set; }
    }
}
