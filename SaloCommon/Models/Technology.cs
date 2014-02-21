using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaloCommon.Models
{
    public class Technology
    {
        /// <summary>
        /// Current tech level
        /// </summary>
        public int level { get; set; }

        /// <summary>
        /// Unsure
        /// </summary>
        public double sv { get; set; }

        /// <summary>
        /// Unsure
        /// </summary>
        public double value { get; set; }

        /// <summary>
        /// Current research added to this
        /// </summary>
        public int research { get; set; }

        /// <summary>
        /// Unsure
        /// </summary>
        public double bv { get; set; }

        /// <summary>
        /// All 144, perhaps base required research
        /// </summary>
        public int brr { get; set; }
    }
}
