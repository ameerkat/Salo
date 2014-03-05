using Salo;
using System;

namespace RandomBot
{
    [BotName("Random Bot", "1.0")]
    [BotDescription("Randomly selects a strategy (econ, war, science) for each production cycle.")]
    public class RandomBot : ISaloBot
    {
        [BotParameter("Ballsiness")]
        const double BALLSINESS = 0.1;

        private static Random rnd = new Random();
        protected Player _player;
        protected IActionHandler _actionHandler;
        protected Configuration _configuration;
        protected StateUtility _stateUtility;

        public Player Me { get { return _player; } }
        public IActionHandler Action { get { return _actionHandler; } }
        public Configuration Configuration { get { return _configuration; } }
        public StateUtility StateUtility { get { return _stateUtility; } }

        public RandomBot(){}

        public void Initialize(Player player, Configuration configuration, IActionHandler actionHandler){
            _player = player;
            _actionHandler = actionHandler;
            _configuration = configuration;
        }

        public void Run(Report game)
        {
            // TODO figure out a better way to handle this
            _stateUtility = new StateUtility(game, Configuration, Me);

            if (game.CurrentTick % Configuration.GetSettingAsInt(Configuration.ConfigurationKeys.ProductionRate) == 0)
            {
                var strat = rnd.Next(0, 3);
                Star star = null;
                int upgradeCost = 0;

                switch (strat)
                {
                    case 0:
                        // economical
                        Action.SetCurrentResearch(Research.Banking);
                        Action.SetNextResearch(Research.Banking);
                        star = StateUtility.GetCheapestUpgradeStar(Star.Upgrade.Economy);
                        upgradeCost = StateUtility.CalculateUpgradeCost(star, Star.Upgrade.Economy);
                        while (Me.Cash >= upgradeCost)
                        {
                            Action.Upgrade(star.Id, Star.Upgrade.Economy);
                            star = StateUtility.GetCheapestUpgradeStar(Star.Upgrade.Economy);
                            upgradeCost = StateUtility.CalculateUpgradeCost(star, Star.Upgrade.Economy);
                        }
                        break;
                    case 1:
                        // militaristic
                        if (Me.Researching == Research.Weapons)
                        {
                            Action.SetNextResearch(Research.Manufacturing);
                        }
                        else
                        {
                            Action.SetNextResearch(Research.Weapons);
                        }

                        // attack
                        var starsInRange = StateUtility.EnemyReachableFromAny();
                        foreach (var starInRange in starsInRange)
                        {
                            var starsThatCanAttack = StateUtility.StarsThatCanReach(starInRange);
                            foreach (var starThatCanAttack in starsThatCanAttack)
                            {
                                if (
                                    (starInRange.IsVisible
                                    && StateUtility.CalculateAttackSuccess(starThatCanAttack, starInRange) > 0)  // only calculate for stars we can see
                                    || rnd.NextDouble() >= (1 - BALLSINESS))
                                {
                                    if (!StateUtility.HasFleet(starThatCanAttack))
                                    {
                                        if (Me.Cash >= Configuration.GetSettingAsInt(Configuration.ConfigurationKeys.FleetBaseCost))
                                        {
                                            Action.BuildFleet(starThatCanAttack.Id);
                                        }
                                        else
                                        {
                                            break; // can't do anything on this planet
                                        }
                                    }

                                    Action.Move(starThatCanAttack.Id, starInRange.Id, starThatCanAttack.Ships);
                                    break;
                                }
                            }
                        }

                        // spend remaining on upgrades
                        star = StateUtility.GetCheapestUpgradeStar(Star.Upgrade.Industry);
                        upgradeCost = StateUtility.CalculateUpgradeCost(star, Star.Upgrade.Industry);
                        while (Me.Cash >= upgradeCost)
                        {
                            Action.Upgrade(star.Id, Star.Upgrade.Industry);
                            star = StateUtility.GetCheapestUpgradeStar(Star.Upgrade.Industry);
                            upgradeCost = StateUtility.CalculateUpgradeCost(star, Star.Upgrade.Industry);
                        }
                        break;
                    case 2:
                        // scientific
                        Action.SetCurrentResearch(Research.Experimentation);
                        Action.SetNextResearch(Research.Experimentation);
                        star = StateUtility.GetCheapestUpgradeStar(Star.Upgrade.Science);
                        upgradeCost = StateUtility.CalculateUpgradeCost(star, Star.Upgrade.Science);
                        while (Me.Cash >= upgradeCost)
                        {
                            Action.Upgrade(star.Id, Star.Upgrade.Science);
                            star = StateUtility.GetCheapestUpgradeStar(Star.Upgrade.Science);
                            upgradeCost = StateUtility.CalculateUpgradeCost(star, Star.Upgrade.Science);
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
