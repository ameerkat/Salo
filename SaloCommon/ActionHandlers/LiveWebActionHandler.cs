using System;
using System.Collections.Generic;

namespace Salo.Live
{
    class LiveWebActionHandler : IActionHandler
    {
        public int UpgradeCost(Salo.Models.Star star, UpgradeType upgradeType)
        {
            throw new NotImplementedException();
        }

        public void Upgrade(Salo.Models.Star star, UpgradeType upgradeType)
        {
            throw new NotImplementedException();
        }

        public bool IsVisible(IEnumerable<Salo.Models.Star> stars, Salo.Models.Star star, Salo.Models.Player player)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Salo.Models.Star> GetVisibleStars(IEnumerable<Salo.Models.Star> stars, Salo.Models.Player player)
        {
            throw new NotImplementedException();
        }

        public bool IsReachableByPlayer(IEnumerable<Salo.Models.Star> stars, Salo.Models.Star star, Salo.Models.Player player)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Salo.Models.Star> GetReachableStars(IEnumerable<Salo.Models.Star> stars, Salo.Models.Player player)
        {
            throw new NotImplementedException();
        }

        public bool IsReachable(IEnumerable<Salo.Models.Star> stars, Salo.Models.Star originStar, Salo.Models.Star destinationStar)
        {
            throw new NotImplementedException();
        }

        public void BuildFleet(Salo.Models.Game game, Salo.Models.Star star)
        {
            throw new NotImplementedException();
        }

        public void Move(Salo.Models.Game game, Salo.Models.Star originStar, Salo.Models.Star destinationStar, int ships)
        {
            throw new NotImplementedException();
        }

        public void MoveAll(Salo.Models.Game game, Salo.Models.Star originStar, Salo.Models.Star destinationStar)
        {
            throw new NotImplementedException();
        }

        public void SetCurrentResearch(Salo.Models.Player player, Technologies tech)
        {
            throw new NotImplementedException();
        }

        public void SetNextResearch(Salo.Models.Player player, Technologies tech)
        {
            throw new NotImplementedException();
        }

        public Salo.Models.Star GetCheapestUpgradeStar(IEnumerable<Salo.Models.Star> stars, Salo.Models.Player player, UpgradeType upgrade)
        {
            throw new NotImplementedException();
        }

        public int CalculateAttackSuccess(Salo.Models.Game game, Salo.Models.Star originStar, Salo.Models.Star destinationStar)
        {
            throw new NotImplementedException();
        }

        public bool HasFleet(Salo.Models.Game game, Salo.Models.Star star)
        {
            throw new NotImplementedException();
        }

        public int FleetCost
        {
            get { throw new NotImplementedException(); }
        }
    }
}
