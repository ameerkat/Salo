using Salo.Models;
using System.Collections.Generic;
using System.Linq;
using Salo.Utility;

namespace Salo
{
    public class ActionHandler : IActionHandler
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
        private const int FleetBaseCost = 25;

        private static int TrueResources(Star star){
            return star.Resources + (star.Owner.Tech.Levels[Technologies.Terraforming] * 5);
        }

        public int UpgradeCost(Star star, UpgradeType upgradeType){
            switch(upgradeType){
                case UpgradeType.Economy:
                    return (int)(2.5 * DevelopmentCostEconomy * (star.Economy + 1) / (TrueResources(star) / 100.0));
                case UpgradeType.Industry:
                    return (int)(5 * DevelopmentCostIndustry * (star.Industry + 1) / (TrueResources(star) / 100.0));
                case UpgradeType.Science:
                    return (int)(20 * DevelopmentCostScience * (star.Science + 1) / (TrueResources(star) / 100.0));
                case UpgradeType.WarpGate:
                    return (int)(100 / (TrueResources(star) / 100.0));
                default:
                    return 0;
            }
        }

        public int FleetCost { get { return FleetBaseCost; } }

        public void Upgrade(Star star, UpgradeType upgradeType)
        {
            if (star.WarpGate && upgradeType == UpgradeType.WarpGate)
            {
                return;
            }

            var upgradeCost = UpgradeCost(star, upgradeType);
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

        public bool IsVisible(IEnumerable<Star> stars, Star star, Player player)
        {
            // TODO Not sure what the actual values are here
            var visibilityRange = BaseVisibilityRange + player.Tech.Levels[Technologies.Scanning] * ScanningMultiplier;
            foreach(var playerStar in stars.Where(x => x.Owner == player)){
                if(playerStar.DistanceTo(star) <= visibilityRange){
                    return true;
                }
            }
            return false;
        }

        public IEnumerable<Star> GetVisibleStars(IEnumerable<Star> stars, Player player)
        {
            return stars.Where(x => IsVisible(stars, x, player));
        }

        private double GetFleetRange(Player player)
        {
            return BaseFleetRange + player.Tech.Levels[Technologies.Range] * PropulsionMultiplier;
        }

        public bool IsReachableByPlayer(IEnumerable<Star> stars, Star star, Player player)
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

        public IEnumerable<Star> GetReachableStars(IEnumerable<Star> stars, Player player)
        {
            return stars.Where(x => IsReachableByPlayer(stars, x, player));
        }

        public bool IsReachable(IEnumerable<Star> stars, Star origin, Star destination)
        {
            return Geometry.CalculateEuclideanDistance(origin, destination) <= GetFleetRange(origin.Owner);
        }

        public void BuildFleet(Game game, Star star)
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

        public void Move(Game game, Star originStar, Star destinationStar, int ships)
        {
            var useFleet = game.Fleets.FirstOrDefault(x => x.CurrentStar != null && x.CurrentStar == originStar);
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

        public void MoveAll(Game game, Star originStar, Star destinationStar)
        {
            Move(game, originStar, destinationStar, (int)originStar.Ships);
        }

        public void SetCurrentResearch(Player player, Technologies tech)
        {
            player.CurrentlyResearching = tech;
        }

        public void SetNextResearch(Player player, Technologies tech)
        {
            player.NextResearching = tech;
        }

        public Star GetCheapestUpgradeStar(IEnumerable<Star> stars, Player player, UpgradeType upgrade)
        {
            var investmentCosts = stars.Where(x => x.Owner == player).Select(x => new KeyValuePair<Star, int>(x, UpgradeCost(x, upgrade)));
            var minInvestmentCost = investmentCosts.Min(x => x.Value);
            return investmentCosts.First(x => x.Value == minInvestmentCost).Key;
        }


        public int CalculateAttackSuccess(Game game, Star originStar, Star destinationStar)
        {
            var attackerWeapons = originStar.Owner.Tech.Levels[Technologies.Weapons];
            var defenderWeapons = destinationStar.Owner.Tech.Levels[Technologies.Weapons] + game.DefenderBonus;
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

        public bool HasFleet(Game game, Star star)
        {
            return game.Fleets.Any(x => x.CurrentStar == star);
        }
    }
}
