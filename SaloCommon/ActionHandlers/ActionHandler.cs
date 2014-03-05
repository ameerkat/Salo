using Salo.Utility;
using System;
using System.Linq;

namespace Salo
{
    public class ActionHandler : IStatefulActionHandler
    {
        protected State _state;
        protected Configuration _configuration;
        protected StateUtility _stateUtility;
        protected readonly int PlayerId;
        public State State { get { return _state; } }
        public Configuration Configuration { get { return _configuration; } }
        public Player Player { get { return State.Players[PlayerId]; } }

        public StateUtility StateUtility
        {
            get { return _stateUtility; }
        }

        public ActionHandler(State state, Configuration configuration, int playerId)
        {
            _state = state;
            _configuration = configuration;
            PlayerId = playerId;
        }

        public void UpdateState(State state)
        {
            this._state = state;
            this._stateUtility = new StateUtility(state, Configuration, Player);
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

            var upgradeCost = StateUtility.CalculateUpgradeCost(star, upgrade);
            if (State.Player(star).Cash >= upgradeCost)
            {
                State.Player(star).Cash -= upgradeCost;
                switch (upgrade)
                {
                    case Star.Upgrade.Economy:
                        star.Economy += 1;
                        break;
                    case Star.Upgrade.Industry:
                        star.Industry += 1;
                        break;
                    case Star.Upgrade.Science:
                        star.Science += 1;
                        break;
                    case Star.Upgrade.WarpGate:
                        star.WarpGate = 1;
                        break;
                    default:
                        return;
                }
            }
        }

        public void BuildFleet(int starId)
        {
            var star = State.Stars[starId];
            var fleetCost = Configuration.GetSettingAsInt(Configuration.ConfigurationKeys.FleetBaseCost);
            if (State.Player(star).Cash >= fleetCost)
            {
                this.State.Player(star).Cash -= fleetCost;
                var id = this.State.GetNextId(typeof (Fleet));
                this.State.Fleets.Add(id, new Fleet()
                {
                    Id = id,
                    Name = NameGenerator.GenerateFleetName(star),
                    OriginStar = null,
                    DestinationStar = null,
                    CurrentStar = star,
                    InTransit = false,
                    DistanceToDestination = 0,
                    Ships = 0, // ships get transfered on movement
                    ToProcess = false,
                    PlayerId = State.Player(star).Id
                });
            }
        }

        public void Move(int originStarId, int destinationStarId, int ships)
        {
            var originStar = State.Stars[originStarId];
            var destinationStar = State.Stars[destinationStarId];
            var useFleet = State.Fleets.Values.FirstOrDefault(x => x.CurrentStar != null && x.CurrentStar == originStar);
            if (useFleet == null)
            {
                throw new FleetRequiredToMoveException();
            }

            if (!this.StateUtility.CanReach(originStar, destinationStar))
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
            useFleet.DistanceToDestination = Geometry.CalculateEuclideanDistance(originStar, destinationStar);
        }

        public void SetCurrentResearch(string research)
        {
            Player.Researching = research;
        }

        public void SetNextResearch(string research)
        {
            Player.ResearchingNext = research;
        }
    }
}
