using System;
using System.Collections.Generic;

namespace Salo
{
    class SimulatorWebActionHandler : IActionHandler
    {
        public int UpgradeCost(Models.Star star, UpgradeType upgradeType)
        {
            throw new NotImplementedException();
        }

        public void Upgrade(Models.Star star, UpgradeType upgradeType)
        {
            throw new NotImplementedException();
        }

        public bool IsVisible(IEnumerable<Models.Star> stars, Models.Star star, Models.Player player)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Models.Star> GetVisibleStars(IEnumerable<Models.Star> stars, Models.Player player)
        {
            throw new NotImplementedException();
        }

        public bool IsReachable(IEnumerable<Models.Star> stars, Models.Star star, Models.Player player)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Models.Star> GetReachableStars(IEnumerable<Models.Star> stars, Models.Player player)
        {
            throw new NotImplementedException();
        }

        public bool IsReachable(IEnumerable<Models.Star> stars, Models.Star origin, Models.Star destination)
        {
            throw new NotImplementedException();
        }

        public void BuildFleet(Models.Game game, Models.Star star)
        {
            throw new NotImplementedException();
        }

        public void Move(Models.Game game, Models.Star originStar, Models.Star destinationStar, int ships)
        {
            throw new NotImplementedException();
        }

        public void MoveAll(Models.Game game, Models.Star originStar, Models.Star destinationStar)
        {
            throw new NotImplementedException();
        }

        public void SetCurrentResearch(Models.Player player, Technologies tech)
        {
            throw new NotImplementedException();
        }

        public void SetNextResearch(Models.Player player, Technologies tech)
        {
            throw new NotImplementedException();
        }

        public Models.Star GetCheapestUpgradeStar(IEnumerable<Models.Star> stars, UpgradeType upgrade)
        {
            throw new NotImplementedException();
        }

        public int CalculateAttackSuccess(Models.Game game, Models.Star origin, Models.Star destination)
        {
            throw new NotImplementedException();
        }

        public bool HasFleet(Models.Game game, Models.Star star)
        {
            throw new NotImplementedException();
        }

        public int FleetCost
        {
            get { throw new NotImplementedException(); }
        }
    }
}
