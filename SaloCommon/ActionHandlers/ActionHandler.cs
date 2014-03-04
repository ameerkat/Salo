using System;
using Salo.Live.Models;
using System.Collections.Generic;
using System.Linq;
using Salo.Utility;

namespace Salo
{
    public class ActionHandler : IActionHandler
    {
        protected State _state;
        protected Configuration _configuration;
        protected readonly int PlayerId;
        public State State { get { return _state; } }
        public Configuration Configuration { get { return _configuration; } }
        public ActionHandler(State state, Configuration configuration, int playerId)
        {
            _state = state;
            _configuration = configuration;
            PlayerId = playerId;
        }

        private static int TrueResources(Star star){
            return star.Resources + (star.Owner.Tech.Levels[Technologies.Terraforming] * 5);
        }

        public void Upgrade(int starId, string upgrade)
        {
            var star = State.Stars[starId];
            if (this.PlayerId != star.PlayerId)
            {
                throw new InsufficientPlayerPermissionsException();
            }

            if (star.WarpGate == 1 && String.Compare(upgrade, Star.Upgrade.WarpGate, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return;
            }

            var upgradeCost = StateUtility.UpgradeCost(star, upgradeType);
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
