using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TritonSimulator.InternalModels;

namespace TritonSimulator
{
    public class RandomBot : ISaloBot
    {
        /*
         * Configuration
         */
        double ballsiness = 0.1;

        protected Player _player;

        public RandomBot(){}

        public void Initialize(Player player){
            _player = player;
        }

        public Player Me { get { return _player; } }

        private static Random rnd = new Random();

        private static Star GetCheapestUpgradeStar(IEnumerable<Star> stars, Actions.UpgradeType upgrade)
        {
            var investmentCosts = stars.Select(x => new KeyValuePair<Star, int>(x, TritonSimulator.Actions.UpgradeCost(x, upgrade)));
            var minInvestmentCost = investmentCosts.Min(x => x.Value);
            return investmentCosts.First(x => x.Value == minInvestmentCost).Key;
        }

        private static int CalculateAttackSuccess(Game game, Star origin, Star destination)
        {
            var attackerWeapons = origin.Owner.Tech.Levels[Technology.Technologies.Weapons];
            var defenderWeapons = destination.Owner.Tech.Levels[Technology.Technologies.Weapons] + game.DefenderBonus;
            int originShips = (int)origin.Ships;
            int destinationShips = (int)destination.Ships;
            while (originShips > 0 && destinationShips > 0)
            {
                // defenders go first
                originShips -= defenderWeapons;
                if (originShips > 0)
                    destinationShips -= attackerWeapons;
            }

            return originShips;
        }

        private static bool HasFleet(Game game, Star star)
        {
            return game.Fleets.Any(x => x.CurrentStar == star);
        }

        public void Run(Game game)
        {
            /*
             * Description
             * Randomly decides strategy for each production cycle
             */

            if (game.ElapsedTicks % game.ProductionRate == 0)
            {
                var strat = rnd.Next(0, 3);
                var myStars = game.Stars.Where(x => x.Owner == Me);
                Star star = null;
                int upgradeCost = 0;

                switch (strat)
                {
                    case 0:
                        // economical
                        Actions.SetCurrentResearch(Me, Technology.Technologies.Banking);
                        Actions.SetNextResearch(Me, Technology.Technologies.Banking);
                        star = GetCheapestUpgradeStar(myStars, Actions.UpgradeType.Economy);
                        upgradeCost = Actions.UpgradeCost(star, Actions.UpgradeType.Economy);
                        while (Me.Cash >= upgradeCost)
                        {
                            Actions.Upgrade(star, Actions.UpgradeType.Economy);
                            star = GetCheapestUpgradeStar(myStars, Actions.UpgradeType.Economy);
                            upgradeCost = Actions.UpgradeCost(star, Actions.UpgradeType.Economy);
                        }
                        break;
                    case 1:
                        // militaristic
                        if (Me.CurrentlyResearching == Technology.Technologies.Weapons)
                        {
                            Actions.SetNextResearch(Me, Technology.Technologies.Manufacturing);
                        }
                        else
                        {
                            Actions.SetNextResearch(Me, Technology.Technologies.Weapons);
                        }

                        // attack
                        var starsInRange = Actions.GetReachableStars(game.Stars, Me).Where(x => x.Owner != Me);
                        foreach (var starInRange in starsInRange)
                        {
                            var starsThatCanAttack = myStars.Where(x => Actions.IsReachable(game.Stars, x, starInRange));
                            foreach (var starThatCanAttack in starsThatCanAttack)
                            {
                                if (
                                    (Actions.IsVisible(game.Stars, starInRange, Me) 
                                    && CalculateAttackSuccess(game, starThatCanAttack, starInRange) > 0)  // only calculate for stars we can see
                                    || rnd.NextDouble() >= (1 - ballsiness))
                                {
                                    if (!HasFleet(game, starThatCanAttack))
                                    {
                                        if (Me.Cash >= Actions.FleetCost)
                                        {
                                            Actions.BuildFleet(game, starThatCanAttack);
                                        }
                                        else
                                        {
                                            break; // can't do anything on this planet
                                        }
                                    }

                                    Actions.MoveAll(game, starThatCanAttack, starInRange);
                                    break;
                                }
                            }
                        }

                        // spend remaining on upgrades
                        star = GetCheapestUpgradeStar(myStars, Actions.UpgradeType.Industry);
                        upgradeCost = Actions.UpgradeCost(star, Actions.UpgradeType.Industry);
                        while (Me.Cash >= upgradeCost)
                        {
                            Actions.Upgrade(star, Actions.UpgradeType.Industry);
                            star = GetCheapestUpgradeStar(myStars, Actions.UpgradeType.Industry);
                            upgradeCost = Actions.UpgradeCost(star, Actions.UpgradeType.Industry);
                        }
                        break;
                    case 2:
                        // science
                        Actions.SetCurrentResearch(Me, Technology.Technologies.Experimentation);
                        Actions.SetNextResearch(Me, Technology.Technologies.Experimentation);
                        star = GetCheapestUpgradeStar(myStars, Actions.UpgradeType.Science);
                        upgradeCost = Actions.UpgradeCost(star, Actions.UpgradeType.Science);
                        while (Me.Cash >= upgradeCost)
                        {
                            Actions.Upgrade(star, Actions.UpgradeType.Science);
                            star = GetCheapestUpgradeStar(myStars, Actions.UpgradeType.Science);
                            upgradeCost = Actions.UpgradeCost(star, Actions.UpgradeType.Science);
                        }
                        break;
                    default:
                        // no op this shouldn't happen
                        break;
                }
            }
        }
    }
}
