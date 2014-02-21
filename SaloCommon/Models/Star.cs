using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaloCommon.Models
{
    public class Star
    {
        /// <summary>
        /// Unique Star ID
        /// </summary>
        public int uid { get; set; }

        /// <summary>
        /// Name of the star
        /// </summary>
        public string n { get; set; }

        /// <summary>
        /// Player Unique ID of the owner of the star
        /// </summary>
        public string puid { get; set; }

        /// <summary>
        /// Is the star Visible
        /// </summary>
        public bool v { get; set; }

        /// <summary>
        /// Star's Y Position
        /// </summary>
        public int y { get; set; }

        /// <summary>
        /// Star's X Position
        /// </summary>
        public int x { get; set; }

        /// <summary>
        /// Unsure
        /// </summary>
        public int c { get; set; }

        /// <summary>
        /// Economy
        /// </summary>
        public int e { get; set; }

        /// <summary>
        /// Industry
        /// </summary>
        public int i { get; set; }

        /// <summary>
        /// Science
        /// </summary>
        public int s { get; set; }

        /// <summary>
        /// Total Resources
        /// </summary>
        public int r { get; set; }

        /// <summary>
        /// Unsure
        /// </summary>
        public int ga { get; set; }

        /// <summary>
        /// Natural Resources
        /// </summary>
        public int nr { get; set; }

        /// <summary>
        /// Number of ships on the star
        /// </summary>
        public int st { get; set; }
    }
}
