using System.Collections.Generic;
using Salo.Models;

namespace Salo
{
    public interface IActionHandler
    {
        int UpgradeCost(Star star, UpgradeType upgradeType);
        void Upgrade(Star star, UpgradeType upgradeType);
        bool IsVisible(IEnumerable<Star> stars, Star star, Player player);
        IEnumerable<Star> GetVisibleStars(IEnumerable<Star> stars, Player player);
        bool IsReachable(IEnumerable<Star> stars, Star star, Player player);
        IEnumerable<Star> GetReachableStars(IEnumerable<Star> stars, Player player);
        bool IsReachable(IEnumerable<Star> stars, Star origin, Star destination);
        void BuildFleet(Game game, Star star);
        void Move(Game game, Star originStar, Star destinationStar, int ships);
        void MoveAll(Game game, Star originStar, Star destinationStar);
        void SetCurrentResearch(Player player, Technologies tech);
        void SetNextResearch(Player player, Technologies tech);
        Star GetCheapestUpgradeStar(IEnumerable<Star> stars, UpgradeType upgrade);
        int CalculateAttackSuccess(Game game, Star origin, Star destination);
        bool HasFleet(Game game, Star star);
        int FleetCost { get; }
    }
}
