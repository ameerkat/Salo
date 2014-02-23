using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TritonSimulator.InternalModels;

namespace TritonSimulator
{
    [BotName("Random Bot", 1.0)]
    [BotDescription("Randomly selects a strategy (econ, war, science) for each production cycle.")]
    public class RandomBot : ISaloBot
    {
        const double BALLSINESS = 0.1;
        private static Random rnd = new Random();
        protected Player _player;
        public Player Me { get { return _player; } }

        public RandomBot(){}

        public void Initialize(Player player){
            _player = player;
        }

        public void Run(Game game)
        {
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
                        star = Actions.GetCheapestUpgradeStar(myStars, Actions.UpgradeType.Economy);
                        upgradeCost = Actions.UpgradeCost(star, Actions.UpgradeType.Economy);
                        while (Me.Cash >= upgradeCost)
                        {
                            Actions.Upgrade(star, Actions.UpgradeType.Economy);
                            star = Actions.GetCheapestUpgradeStar(myStars, Actions.UpgradeType.Economy);
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
                                    && Actions.CalculateAttackSuccess(game, starThatCanAttack, starInRange) > 0)  // only calculate for stars we can see
                                    || rnd.NextDouble() >= (1 - BALLSINESS))
                                {
                                    if (!Actions.HasFleet(game, starThatCanAttack))
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
                        star = Actions.GetCheapestUpgradeStar(myStars, Actions.UpgradeType.Industry);
                        upgradeCost = Actions.UpgradeCost(star, Actions.UpgradeType.Industry);
                        while (Me.Cash >= upgradeCost)
                        {
                            Actions.Upgrade(star, Actions.UpgradeType.Industry);
                            star = Actions.GetCheapestUpgradeStar(myStars, Actions.UpgradeType.Industry);
                            upgradeCost = Actions.UpgradeCost(star, Actions.UpgradeType.Industry);
                        }
                        break;
                    case 2:
                        // science
                        Actions.SetCurrentResearch(Me, Technology.Technologies.Experimentation);
                        Actions.SetNextResearch(Me, Technology.Technologies.Experimentation);
                        star = Actions.GetCheapestUpgradeStar(myStars, Actions.UpgradeType.Science);
                        upgradeCost = Actions.UpgradeCost(star, Actions.UpgradeType.Science);
                        while (Me.Cash >= upgradeCost)
                        {
                            Actions.Upgrade(star, Actions.UpgradeType.Science);
                            star = Actions.GetCheapestUpgradeStar(myStars, Actions.UpgradeType.Science);
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
