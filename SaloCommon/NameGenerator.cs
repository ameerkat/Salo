using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salo
{
    public static class NameGenerator
    {
        public static string GenerateFleetName(TritonSimulator.InternalModels.Star originStar){
            return "Ships of " + originStar;
        }
    }
}
