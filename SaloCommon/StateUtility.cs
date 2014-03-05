using System;
using System.Collections.Generic;
using System.Linq;

namespace Salo
{
    public static class Geometry
    {
        public static double CalculateEuclideanDistance(double x1, double x2, double y1, double y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }

        public static double CalculateEuclideanDistance(Star star1, Star star2)
        {
            return CalculateEuclideanDistance(star1.X, star2.X, star1.Y, star2.Y);
        }

        public static double DistanceTo(this Star origin, Star star)
        {
            return CalculateEuclideanDistance(origin, star);
        }

        public static double CalculateClusterMeanDistance(List<Star> cluster, double x, double y)
        {
            return cluster.Average((star) => CalculateEuclideanDistance(star.X, x, star.Y, y));
        }

        public static double DegreesToRadians(double a)
        {
            return a * Math.PI / 180;
        }
    }

    public class StateUtility
    {
        public readonly State State;
        public readonly Configuration Configuration;
        public readonly Player Player;
        public StateUtility(State state, Configuration configuration, Player player)
        {
            this.State = state;
            this.Configuration = configuration;
            this.Player = player;
        }

        public StateUtility(Report report, Configuration configuration, Player player)
        {
            State = new State()
            {
                CurrentTick = report.CurrentTick,
                Fleets = report.Fleets,
                IsOver = report.IsOver,
                IsPaused = report.IsPaused,
                IsStarted = report.IsStarted,
                Players = report.Players,
                Stars = report.Stars
            };

            Configuration = configuration;
            Player = player;
        }

        public int CalculateUpgradeCost(Star star, string upgrade)
        {
            switch(upgrade){
                case Star.Upgrade.Economy:
                    return (int)(2.5 * Configuration.GetSettingAsInt(Configuration.ConfigurationKeys.DevelopmentCostEconomy) * (star.Economy + 1) / (star.TotalResources / 100.0));
                case Star.Upgrade.Industry:
                    return (int)(5 * Configuration.GetSettingAsInt(Configuration.ConfigurationKeys.DevelopmentCostIndustry) * (star.Industry + 1) / (star.TotalResources / 100.0));
                case Star.Upgrade.Science:
                    return (int)(20 * Configuration.GetSettingAsInt(Configuration.ConfigurationKeys.DevelopmentCostScience) * (star.Science + 1) / (star.TotalResources / 100.0));
                case Star.Upgrade.WarpGate:
                    return (int)(100 / (star.TotalResources / 100.0));
                default:
                    return 0;
            }
        }

        public double GetFleetRange()
        {
            return Configuration.GetSettingAsDouble(Configuration.ConfigurationKeys.BaseFleetRange) + (Player.Technology[Research.Propulsion].Level * Configuration.GetSettingAsDouble(Configuration.ConfigurationKeys.BaseFleetRange));
        }

        public bool CanReach(Star origin, Star destination)
        {
            var fleetRange = GetFleetRange();
            return origin.DistanceTo(destination) <= fleetRange;
        }

        /// <summary>
        /// Returns all of your stars that can reach the target star
        /// </summary>
        /// <param name="star"></param>
        /// <returns></returns>
        public IEnumerable<Star> StarsThatCanReach(Star star)
        {
            return State.Stars.Where(s => s.Value.PlayerId == Player.Id && CanReach(s.Value, star)).Select(x => x.Value);
        }

        public IEnumerable<Star> EnemyReachableFromAny()
        {
            return State.Stars.Where(x => x.Value.PlayerId != Player.Id && StarsThatCanReach(x.Value).Any()).Select(x => x.Value);
        }

        public Star GetCheapestUpgradeStar(string upgrade)
        {
            var investmentCosts = State.Stars.Values.Where(x => x.PlayerId == Player.Id).Select(x => new KeyValuePair<Star, int>(x, CalculateUpgradeCost(x, upgrade)));
            var minInvestmentCost = investmentCosts.Min(x => x.Value);
            return investmentCosts.First(x => x.Value == minInvestmentCost).Key;
        }

        public int CalculateAttackSuccess(Star originStar, Star destinationStar)
        {
            var attackerWeapons = State.Player(originStar).Technology[Research.Weapons].Level;
            var defenderWeapons = State.Player(destinationStar).Technology[Research.Weapons].Level
                + this.Configuration.GetSettingAsInt(Configuration.ConfigurationKeys.DefenderBonusWeapons);
            int originShips = (int)originStar.Ships;
            int destinationShips = (int)destinationStar.Ships;
            while (originShips > 0 && destinationShips > 0)
            {
                // defenders go first
                originShips -= defenderWeapons;
                if (originShips > 0)
                    destinationShips -= attackerWeapons;
            }

            return originShips;
        }

        public bool HasFleet(Star star)
        {
            return State.Fleets.Values.Any(x => x.CurrentStar == star);
        }
    }
}
