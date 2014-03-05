using System;
using System.Collections.Generic;
using System.Linq;

namespace Salo
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

        private static bool IsClustersOverlap(List<List<Star>> clusters, List<Star> cluster, double threshold)
        {
            var homeStar = cluster.Where(star => star.IsHomeStar).First();
            foreach (var c in clusters)
            {
                if (Salo.Geometry.CalculateClusterMeanDistance(c, homeStar.X, homeStar.Y) < threshold)
                    return true;
            }
            return false;
        }

        public static State GenerateMap(
            int players,
            int startingStars,
            int starsPerPlayer,
            Configuration configuration,
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
            State state = new State();

            var clusters = new List<List<Star>>();

            double previousClusterCenterX = 0.0;
            double previousClusterCenterY = 0.0;

            for (int i = 0; i < players; ++i)
            {
                var Cluster = new List<Star>();
                var HomeStar = new Star()
                {
                    Id = state.GetNextId(typeof(Star)),
                    X = startingScale / 2.0, // position at center
                    Y = startingScale / 2.0,
                    IsHomeStar = true,
                    IsStartingStar = true,
                    PlayerId = i,
                    NaturalResources = 50, // all home stars start at 50,
                    // all players start with level 1 terraforming, todo make this more dynamic
                    TotalResources = configuration.GetSettingAsInt(Configuration.ConfigurationKeys.TerraformingResourceCoefficient) + 50
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

                    var resource = ResourceGenerator(resources);
                    var Star = new Star()
                    {
                        Id = state.GetNextId(typeof(Star)),
                        X = x,
                        Y = y,
                        IsHomeStar = false,
                        IsStartingStar = true,
                        PlayerId = i,
                        NaturalResources = resource,
                        // all players start with level 1 terraforming, todo make this more dynamic
                        TotalResources = configuration.GetSettingAsInt(Configuration.ConfigurationKeys.TerraformingResourceCoefficient) + resource
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

                    var resource = ResourceGenerator(resources);
                    var Star = new Star()
                    {
                        Id = state.GetNextId(typeof(Star)),
                        X = x,
                        Y = y,
                        IsHomeStar = false,
                        IsStartingStar = false,
                        PlayerId = i,
                        NaturalResources = resource,
                        // all players start with level 1 terraforming, todo make this more dynamic
                        TotalResources = configuration.GetSettingAsInt(Configuration.ConfigurationKeys.TerraformingResourceCoefficient) + resource
                    };
                    Cluster.Add(Star);
                }

                // transform cluster

                // put homestar in 0 degree position above origin
                foreach (var star in Cluster)
                {
                    star.X -= startingScale / 2;
                    star.Y += startingDistances[startingDistance];
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
                        var newx = (star.X * Math.Cos(rotationAngle)) - (star.Y * Math.Sin(rotationAngle));
                        var newy = (star.X * Math.Sin(rotationAngle)) - (star.Y * Math.Cos(rotationAngle));
                        star.X = newx + previousClusterCenterX;
                        star.Y = newy + previousClusterCenterY;
                    }

                    if (IsClustersOverlap(clusters, Cluster, startingDistances[startingDistance]))
                    {
                        //undo
                        foreach (var star in Cluster)
                        {
                            star.X -= previousClusterCenterX;
                            star.Y -= previousClusterCenterY;
                        }
                        repeat = true;
                    }
                    else
                    {
                        repeat = false;
                    }
                } while (repeat);

                previousClusterCenterX = HomeStar.X;
                previousClusterCenterY = HomeStar.Y;
                
                clusters.Add(Cluster);
                foreach (var star in Cluster)
                {
                    state.Stars.Add(star.Id, star);
                }
            }

            // make everything positive
            double minX = state.Stars.Values.Min(star => star.X);
            double minY = state.Stars.Values.Min(star => star.Y);
            foreach (var star in state.Stars.Values)
            {
                if(minX < 0)
                    star.X += Math.Abs(minX);
                if(minY < 0)
                    star.Y += Math.Abs(minY);
            }

            return state;
        }
    }
}
