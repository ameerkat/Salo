using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salo.Models
{
    public class Player
    {
        public int Id { get; set; }
        public Technology Tech { get; set; }
        public Technologies CurrentlyResearching { get; set; }
        public Technologies NextResearching { get; set; }
        public string Name { get; set; }
        public int Cash { get; set; }
    }
}
