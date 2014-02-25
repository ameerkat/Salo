using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salo.Utility
{
    public static class NameGenerator
    {
        public static string GenerateFleetName(Salo.Models.Star originStar){
            return "Ships of " + originStar;
        }
    }
}
