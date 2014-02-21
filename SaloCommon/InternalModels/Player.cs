using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TritonSimulator.InternalModels
{
    public class Player
    {
        public int Id { get; set; }
        public Technology Tech { get; set; }
        public Technology.Technologies CurrentlyResearching { get; set; }
        public Technology.Technologies NextResearching { get; set; }
        public string Name { get; set; }
        public int Cash { get; set; }
    }
}
