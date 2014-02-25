using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Salo;
using Salo.Models;

namespace RandomBot
{
    [BotName("Random Bot", 1.0)]
    [BotDescription("Randomly selects a strategy (econ, war, science) for each production cycle.")]
    public class RandomBot : ISaloBot
    {
        const double BALLSINESS = 0.1;
        private static Random rnd = new Random();
        protected Player _player;
        protected IActionHandler _actionHandler;
        public Player Me { get { return _player; } }
        public IActionHandler Action { get { return _actionHandler; } }

        public RandomBot(){}

        public void Initialize(Player player, IActionHandler actionHandler){
            _player = player;
            _actionHandler = actionHandler;
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
                        Action.SetCurrentResearch(Me, Technologies.Banking);
                        Action.SetNextResearch(Me, Technologies.Banking);
                        star = Action.GetCheapestUpgradeStar(myStars, UpgradeType.Economy);
                        upgradeCost = Action.UpgradeCost(star, UpgradeType.Economy);
                        while (Me.Cash >= upgradeCost)
                        {
                            Action.Upgrade(star, UpgradeType.Economy);
                            star = Action.GetCheapestUpgradeStar(myStars, UpgradeType.Economy);
                            upgradeCost = Action.UpgradeCost(star, UpgradeType.Economy);
                        }
                        break;
                    case 1:
                        // militaristic
                        if (Me.CurrentlyResearching == Technologies.Weapons)
                        {
                            Action.SetNextResearch(Me, Technologies.Manufacturing);
                        }
                        else
                        {
                            Action.SetNextResearch(Me, Technologies.Weapons);
                        }

                        // attack
                        var starsInRange = Action.GetReachableStars(game.Stars, Me).Where(x => x.Owner != Me);
                        foreach (var starInRange in starsInRange)
                        {
                            var starsThatCanAttack = myStars.Where(x => Action.IsReachable(game.Stars, x, starInRange));
                            foreach (var starThatCanAttack in starsThatCanAttack)
                            {
                                if (
                                    (Action.IsVisible(game.Stars, starInRange, Me)
                                    && Action.CalculateAttackSuccess(game, starThatCanAttack, starInRange) > 0)  // only calculate for stars we can see
                                    || rnd.NextDouble() >= (1 - BALLSINESS))
                                {
                                    if (!Action.HasFleet(game, starThatCanAttack))
                                    {
                                        if (Me.Cash >= Action.FleetCost)
                                        {
                                            Action.BuildFleet(game, starThatCanAttack);
                                        }
                                        else
                                        {
                                            break; // can't do anything on this planet
                                        }
                                    }

                                    Action.MoveAll(game, starThatCanAttack, starInRange);
                                    break;
                                }
                            }
                        }

                        // spend remaining on upgrades
                        star = Action.GetCheapestUpgradeStar(myStars, UpgradeType.Industry);
                        upgradeCost = Action.UpgradeCost(star, UpgradeType.Industry);
                        while (Me.Cash >= upgradeCost)
                        {
                            Action.Upgrade(star, UpgradeType.Industry);
                            star = Action.GetCheapestUpgradeStar(myStars, UpgradeType.Industry);
                            upgradeCost = Action.UpgradeCost(star, UpgradeType.Industry);
                        }
                        break;
                    case 2:
                        // scientific
                        Action.SetCurrentResearch(Me, Technologies.Experimentation);
                        Action.SetNextResearch(Me, Technologies.Experimentation);
                        star = Action.GetCheapestUpgradeStar(myStars, UpgradeType.Science);
                        upgradeCost = Action.UpgradeCost(star, UpgradeType.Science);
                        while (Me.Cash >= upgradeCost)
                        {
                            Action.Upgrade(star, UpgradeType.Science);
                            star = Action.GetCheapestUpgradeStar(myStars, UpgradeType.Science);
                            upgradeCost = Action.UpgradeCost(star, UpgradeType.Science);
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
