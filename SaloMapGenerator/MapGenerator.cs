using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TritonSimulator.InternalModels.Map;

namespace SaloMapGenerator
{
    public static class MapGenerator
    {
        private const double startingScale = 1.0;
        private static Random rnd = new Random();
        private const double maxStartingRange = 0.125;

        public enum GalaxyType {
            RandomHex, // only this is supported right now
            // Circular
        }

        public enum ResourcesLevel {
            Sparse,
            Standard,
            Plentiful
        }

        public enum StartingDistance {
            Close,
            Medium,
            Far
        }

        /*
         * Settings
         */
        private static Dictionary<ResourcesLevel, int> resourcesScale = new Dictionary<ResourcesLevel, int>(){
                {ResourcesLevel.Plentiful, 60},
                {ResourcesLevel.Standard, 45},
                {ResourcesLevel.Sparse, 30},
            };

        private static Dictionary<StartingDistance, double> startingDistances = new Dictionary<StartingDistance, double>(){
                {StartingDistance.Close, 1},
                {StartingDistance.Medium, 0.5},
                {StartingDistance.Far, 1},
            };

        private static int ResourceGenerator(ResourcesLevel resourcesLevel)
        {
            var baseResourcesModifier = rnd.NextDouble();
            var resources = (int)(resourcesScale[resourcesLevel] * baseResourcesModifier);
            return resources == 0 ? 1 : resources;
        }

        private static bool IsClustersOverlap(List<List<MapStar>> clusters, List<MapStar> cluster, double threshold)
        {
            var homeStar = cluster.Where(star => star.isHomeStar).First();
            foreach (var c in clusters)
            {
                if (Salo.Geometry.CalculateClusterMeanDistance(c, homeStar.x, homeStar.y) < threshold)
                    return true;
            }
            return false;
        }

        public static Map GenerateMap(
            int players,
            int startingStars,
            int starsPerPlayer,
            GalaxyType galaxyType = GalaxyType.RandomHex, 
            ResourcesLevel resources = ResourcesLevel.Standard,
            StartingDistance startingDistance = StartingDistance.Medium)
        {
            /*
             * Approach
             * Generate a starting cluster for each player
             * Ensure that the player has x% of stars reachable with level 1 tech
             * Ensure that the stars are appropriately clustered (within a range of x to y mean distance from one another)
             * Place the cluster x distance (based on starting distance) from center of other clusters at random 60 degree angle
             * (random 60 degree angle because of RandomHex galaxy type)
             */
            int global_star_counter = 0;
            Map map = new Map(){
                stars = new List<MapStar>()
            };

            var clusters = new List<List<MapStar>>();

            double previousClusterCenterX = 0.0;
            double previousClusterCenterY = 0.0;

            for (int i = 0; i < players; ++i)
            {
                var Cluster = new List<MapStar>();
                var HomeStar = new MapStar()
                {
                    id = global_star_counter++,
                    x = startingScale / 2.0, // position at center
                    y = startingScale / 2.0,
                    isHomeStar = true,
                    isStartingStar = true,
                    player = i,
                    resources = 50 // all home stars start at 50
                };
                Cluster.Add(HomeStar);

                // generate cluster of starting stars
                for (int j = 1; j < startingStars; ++j)
                {
                    double x, y, clusterMeanDistance;
                    do
                    {
                        x = rnd.NextDouble() * startingScale;
                        y = rnd.NextDouble() * startingScale;
                        clusterMeanDistance = Salo.Geometry.CalculateClusterMeanDistance(Cluster, x, y);
                    } while (clusterMeanDistance < (maxStartingRange / 2) &&
                        clusterMeanDistance > (maxStartingRange));
                    var Star = new MapStar()
                    {
                        id = global_star_counter++,
                        x = x,
                        y = y,
                        isHomeStar = false,
                        isStartingStar = true,
                        player = i,
                        resources = ResourceGenerator(resources)
                    };
                    Cluster.Add(Star);
                }

                // generate remaining stars
                for (int k = startingStars+1; k < starsPerPlayer; ++k)
                {
                    double x, y, clusterMeanDistance;
                    do
                    {
                        x = rnd.NextDouble() * startingScale;
                        y = rnd.NextDouble() * startingScale;
                        clusterMeanDistance = Salo.Geometry.CalculateClusterMeanDistance(Cluster, x, y);
                    } while (clusterMeanDistance < maxStartingRange);
                    var Star = new MapStar()
                    {
                        id = global_star_counter++,
                        x = x,
                        y = y,
                        isHomeStar = false,
                        isStartingStar = false,
                        player = i,
                        resources = ResourceGenerator(resources)
                    };
                    Cluster.Add(Star);
                }

                // transform cluster

                // put homestar in 0 degree position above origin
                foreach (var star in Cluster)
                {
                    star.x -= startingScale / 2;
                    star.y += startingDistances[startingDistance];
                }

                var repeat = false;
                var tried = 0;
                do
                {
                    // rotate by 60 degrees each retry
                    var angleToTry = repeat ? 1 : rnd.Next(0, 5);
                    if (tried++ > 6)
                    {
                        throw new Exception();
                    }

                    var rotationAngle = Salo.Geometry.DegreesToRadians(60 * angleToTry);
                    
                    foreach (var star in Cluster)
                    {
                        var newx = (star.x * Math.Cos(rotationAngle)) - (star.y * Math.Sin(rotationAngle));
                        var newy = (star.x * Math.Sin(rotationAngle)) - (star.y * Math.Cos(rotationAngle));
                        star.x = newx + previousClusterCenterX;
                        star.y = newy + previousClusterCenterY;
                    }

                    if (IsClustersOverlap(clusters, Cluster, startingDistances[startingDistance]))
                    {
                        //undo
                        foreach (var star in Cluster)
                        {
                            star.x -= previousClusterCenterX;
                            star.y -= previousClusterCenterY;
                        }
                        repeat = true;
                    }
                    else
                    {
                        repeat = false;
                    }
                } while (repeat);

                previousClusterCenterX = HomeStar.x;
                previousClusterCenterY = HomeStar.y;
                
                clusters.Add(Cluster);
                map.stars.AddRange(Cluster);
            }

            // make everything positive
            double minX = map.stars.Min(star => star.x);
            double minY = map.stars.Min(star => star.y);
            foreach (var star in map.stars)
            {
                if(minX < 0)
                    star.x += Math.Abs(minX);
                if(minY < 0)
                    star.y += Math.Abs(minY);
            }

            return map;
        }
    }
}
