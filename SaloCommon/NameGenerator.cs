using Salo.Live.Models;

namespace Salo.Utility
{
    public static class NameGenerator
    {
        public static string GenerateFleetName(Star originStar){
            return "Ships of " + originStar.Name;
        }
    }
}
