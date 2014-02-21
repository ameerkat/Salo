using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TritonSimulator.InternalModels
{
    public class Technology
    {
        public enum Technologies {
            Weapons = 0, 
            Banking, 
            Scanning, 
            Range, 
            Terraforming, 
            Experimentation, 
            Manufacturing
        }

        public Dictionary<Technologies, int> Levels = new Dictionary<Technologies, int>(){
            { Technologies.Banking, 1 },
            { Technologies.Experimentation, 1 },
            { Technologies.Manufacturing, 1 },
            { Technologies.Range, 1 },
            { Technologies.Scanning, 1 },
            { Technologies.Terraforming, 1 },
            { Technologies.Weapons, 1 }
        };

        public Dictionary<Technologies, int> Values = new Dictionary<Technologies, int>(){
            { Technologies.Banking, 0 },
            { Technologies.Experimentation, 0 },
            { Technologies.Manufacturing, 0 },
            { Technologies.Range, 0 },
            { Technologies.Scanning, 0 },
            { Technologies.Terraforming, 0 },
            { Technologies.Weapons, 0 }
        };
    }
}
