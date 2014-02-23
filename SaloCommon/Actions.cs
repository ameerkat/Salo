using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TritonSimulator.InternalModels;
using Salo;

namespace TritonSimulator
{
    public static class Actions
    {
        /*
         * Configuration
         */
        private const int DevelopmentCostEconomy = 2;
        private const int DevelopmentCostIndustry = 2;
        private const int DevelopmentCostScience = 2;
        private const double BaseVisibilityRange = 0.75;
        private const double ScanningMultiplier = 0.1;
        private const double BaseFleetRange = 1;
        private const double PropulsionMultiplier = 0.15;
        public const int FleetCost = 25;

        public enum UpgradeType {
            Economy,
            Industry,
            Science,
            WarpGate
        }

        public static int TrueResources(this Star star){
            return star.Resources + (star.Owner.Tech.Levels[Technology.Technologies.Terraforming] * 5);
        }

        public static int UpgradeCost(this Star star, UpgradeType upgradeType){
            switch(upgradeType){
                case UpgradeType.Economy:
                    return (int)(2.5 * DevelopmentCostEconomy * (star.Economy + 1) / (star.TrueResources() / 100.0));
                case UpgradeType.Industry:
                    return (int)(5 * DevelopmentCostIndustry * (star.Industry + 1) / (star.TrueResources() / 100.0));
                case UpgradeType.Science:
                    return (int)(20 * DevelopmentCostScience * (star.Science + 1) / (star.TrueResources() / 100.0));
                case UpgradeType.WarpGate:
                    return (int)(100 / (star.TrueResources() / 100.0));
                default:
                    return 0;
            }
        }

        public static void Upgrade(this Star star, UpgradeType upgradeType)
        {
            if (star.WarpGate && upgradeType == UpgradeType.WarpGate)
            {
                return;
            }

            var upgradeCost = star.UpgradeCost(upgradeType);
            if (star.Owner.Cash >= upgradeCost)
            {
                star.Owner.Cash -= upgradeCost;
                switch (upgradeType)
                {
                    case UpgradeType.Economy:
                        star.Economy += 1;
                        break;
                    case UpgradeType.Industry:
                        star.Industry += 1;
                        break;
                    case UpgradeType.Science:
                        star.Science += 1;
                        break;
                    case UpgradeType.WarpGate:
                        star.WarpGate = true;
                        break;
                    default:
                        return;
                }
            }
        }

        public static bool IsVisible(IEnumerable<Star> stars, Star star, Player player)
        {
            // TODO Not sure what the actual values are here
            var visibilityRange = BaseVisibilityRange + player.Tech.Levels[Technology.Technologies.Scanning] * ScanningMultiplier;
            foreach(var playerStar in stars.Where(x => x.Owner == player)){
                if(playerStar.DistanceTo(star) <= visibilityRange){
                    return true;
                }
            }
            return false;
        }

        public static IEnumerable<Star> GetVisibleStars(IEnumerable<Star> stars, Player player)
        {
            return stars.Where(x => IsVisible(stars, x, player));
        }

        private static double GetFleetRange(Player player)
        {
            return BaseFleetRange + player.Tech.Levels[Technology.Technologies.Range] * PropulsionMultiplier;
        }

        public static bool IsReachable(IEnumerable<Star> stars, Star star, Player player)
        {
            var fleetRange = GetFleetRange(player);
            foreach (var playerStar in stars.Where(x => x.Owner == player))
            {
                if (playerStar.DistanceTo(star) <= fleetRange)
                {
                    return true;
                }
            }
            return false;
        }

        public static IEnumerable<Star> GetReachableStars(IEnumerable<Star> stars, Player player)
        {
            return stars.Where(x => IsReachable(stars, x, player));
        }

        public static bool IsReachable(IEnumerable<Star> stars, Star origin, Star destination)
        {
            return Geometry.CalculateEuclideanDistance(origin, destination) <= GetFleetRange(origin.Owner);
        }

        public static void BuildFleet(Game game, Star star)
        {
            if (star.Owner.Cash >= FleetCost)
            {
                star.Owner.Cash -= FleetCost;
                game.Fleets.Add(new Fleet()
                {
                    Id = game.fleetId++,
                    Name = NameGenerator.GenerateFleetName(star),
                    OriginStar = null,
                    DestinationStar = null,
                    CurrentStar = star,
                    InTransit = false,
                    DistanceToDestination = 0,
                    Ships = 0, // ships get transfered on movement
                    ToProcess = false,
                    Owner = star.Owner
                });
            }
        }

        public static void Move(Game game, Star originStar, Star destinationStar, int ships)
        {
            var useFleet = game.Fleets.Where(x => x.CurrentStar != null && x.CurrentStar == originStar).FirstOrDefault();
            if (useFleet == null)
            {
                throw new FleetRequiredToMoveException();
            }

            var distance = Geometry.CalculateEuclideanDistance(originStar, destinationStar);
            if (distance > GetFleetRange(originStar.Owner))
            {
                throw new InsufficientRangeException();
            }

            if (ships > originStar.Ships)
            {
                throw new InsufficientShipsException();
            }

            useFleet.Ships += ships;
            originStar.Ships -= ships;
            useFleet.OriginStar = originStar;
            useFleet.DestinationStar = destinationStar;
            useFleet.CurrentStar = null;
            useFleet.InTransit = true;
            useFleet.DistanceToDestination = distance;
        }

        public static void MoveAll(Game game, Star originStar, Star destinationStar)
        {
            Move(game, originStar, destinationStar, (int)originStar.Ships);
        }

        public static void SetCurrentResearch(Player player, Technology.Technologies tech)
        {
            player.CurrentlyResearching = tech;
        }

        public static void SetNextResearch(Player player, Technology.Technologies tech)
        {
            player.NextResearching = tech;
        }
    }
}
