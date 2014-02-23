using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salo
{
    public static class Geometry
    {
        public static double CalculateEuclideanDistance(double x1, double x2, double y1, double y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }

        public static double CalculateEuclideanDistance(TritonSimulator.InternalModels.Star star1, TritonSimulator.InternalModels.Star star2)
        {
            return CalculateEuclideanDistance(star1.x, star2.x, star1.y, star2.y);
        }

        public static double DistanceTo(this TritonSimulator.InternalModels.Star origin, TritonSimulator.InternalModels.Star star)
        {
            return CalculateEuclideanDistance(origin, star);
        }

        public static double CalculateClusterMeanDistance(List<TritonSimulator.InternalModels.Map.MapStar> cluster, double x, double y)
        {
            return cluster.Average((star) => CalculateEuclideanDistance(star.x, x, star.y, y));
        }

        public static double DegreesToRadians(double a)
        {
            return a * Math.PI / 180;
        }
    }
}
