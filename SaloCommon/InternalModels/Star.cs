using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TritonSimulator.InternalModels
{
    public class Star
    {
        public int Id { get; set; }
        public Player Owner { get; set; }
        public string Name { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        /// <summary>
        /// This gets rounded to int at combat
        /// </summary>
        public double Ships { get; set; }
        public int Economy { get; set; }
        public int Industry { get; set; }
        public int Science { get; set; }
        public int Resources { get; set; }
        public bool WarpGate { get; set; }
    }
}
