using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Salo;
using Salo.Live.Models;

namespace SaloSimulator
{
    public static class SimulatorCore
    {
        private static Random rnd = new Random();

        /// <summary>
        /// Returns null if no winner, returns Player if game is won
        /// </summary>
        private static bool CheckForWinner(State state, Configuration configuration)
        {
            var requiredStars = (double)(configuration.GetSettingAsDouble(Configuration.ConfigurationKeys.StarsForVictoryPercent) * state.Stars.Count);
            var playerStars = CalculateStarByPlayer(state, (x) => 1);
            return playerStars.Any(x => x.Value >= requiredStars);
        }

        private static Dictionary<Player, int> InitializePlayerIntDict(State state)
        {
            return state.Players.ToDictionary(player => player.Value, player => 0);
        }

        /// <summary>
        /// Calculates star stats by player
        /// </summary>
        private static Dictionary<Player, int> CalculateStarByPlayer(State state, Func<Star, int> selector)
        {
            var statByPlayer = InitializePlayerIntDict(state);
            foreach (var star in state.Stars)
            {
                // TODO figure out how game actually represents this
                // however not critical to internal workings unless we want our server to plug with
                // the actual game client
                if (star.Value.PlayerId != -1)
                {
                    statByPlayer[state.Players[star.Value.PlayerId]] += selector(star.Value);
                }
            }
            return statByPlayer;
        }

        private static int CalculateTotalResearchAmountRequired(int level, int baseRequiredResearch)
        {
            var amount = 0;
            for (int l = 0; l < level; ++l)
            {
                amount += baseRequiredResearch*l;
            }
            return amount;
        }

        /// <summary>
        /// Returns true if the research was upgraded a level
        /// (used to determine if should move to next research)
        /// <returns></returns>
        private static bool ConsolidateResearch(Configuration configuration, Player player, string technology)
        {
            var wentUpALevel = false;
            while (player.Technology[technology].Amount >= 
                (CalculateTotalResearchAmountRequired(player.Technology[technology].Level + 1, 
                configuration.GetSettingAsInt(Configuration.ConfigurationKeys.BaseRequiredResearch))))
            {
                player.Technology[technology].Level += 1;
                wentUpALevel = true;
            }
            return wentUpALevel;
        }

        public static void ProcessTick(State state, Configuration configuration, IEnumerable<ISaloBot> bots)
        {
            /*
             * Overall Approach
             * ---
             * All the stars are points. Main game loop runs, calculates a bunch of stuff then sleeps until the next tick.
             * Things to Calculate:
             * * Production
             * * Industry Accumulation
             * * Position Updates
             * * Research
             * * Conflict
             * * Bots Makes New Moves
             */
            
            // Check win conditions
            if (CheckForWinner(state, configuration))
            {
                state.IsOver = true;
                return;
            }

            // Each pass of loop is one tick, loop should sleep to synch ticks
            state.CurrentTick += 1;

            // Calculate Production
            // MoneyPerEcon * Total Economy + 75 * Banking
            // Add research bonus from experimentation
            if (state.CurrentTick % configuration.GetSettingAsInt(Configuration.ConfigurationKeys.ProductionRate) == 0)
            {
                var playerEconomy = CalculateStarByPlayer(state, (x) => x.Economy);
                foreach (var player in state.Players)
                {
                    player.Value.Cash += (playerEconomy[player.Value] * configuration.GetSettingAsInt(Configuration.ConfigurationKeys.CashPerEconomoy));
                    // Calculate banking bonus
                    player.Value.Cash += (player.Value.Technology[Research.Banking].Level * configuration.GetSettingAsInt(Configuration.ConfigurationKeys.CashPerBanking));
                    var researchToAdd = player.Value.Technology[Research.Experimentation].Level * configuration.GetSettingAsInt(Configuration.ConfigurationKeys.ResearchPerExperimentation);
                    List<string> research = new List<string>(){Research.Banking, Research.Experimentation, Research.Manufacturing, Research.Propulsion, Research.Scanning, Research.Terraforming, Research.Weapons};
                    string randomResearch = research[rnd.Next(0, 7)]; // returns 0,1,...,6
                    player.Value.Technology[randomResearch].Amount += researchToAdd;
                    // Resolve research levels before continuing
                    var upgraded = ConsolidateResearch(configuration, player.Value, randomResearch);
                    if(upgraded && randomResearch == player.Value.Researching){
                        player.Value.Researching = player.Value.ResearchingNext;
                    }

                    if (upgraded && randomResearch == Research.Terraforming)
                    {
                        foreach (var star in state.Stars.Where(x => x.Value.PlayerId == player.Value.Id))
                        {
                            star.Value.TotalResources =
                                star.Value.NaturalResources +
                                (state.Players[star.Value.PlayerId].Technology[Research.Terraforming].Level *
                                 configuration.GetSettingAsInt(
                                     Configuration.ConfigurationKeys.TerraformingResourceCoefficient));
                        }
                    }
                }
            }

            // Calculate Industry
            // Stars produce Y*(X+5) every production # of ticks
            // X is tech level, Y is industry
            // We just calculate each tick and store partial ships
            // Round down when calculating for Combat
            foreach (var star in state.Stars)
            {
                double incrementValuePerProduction = 
                    star.Value.Industry *
                    (state.Players[star.Value.PlayerId].Technology[Research.Manufacturing].Level * configuration.GetSettingAsInt(Configuration.ConfigurationKeys.ManufacturingLevelCoeffienct));
                var increment = incrementValuePerProduction / (double)configuration.GetSettingAsInt(Configuration.ConfigurationKeys.ProductionRate);
                star.Value.ShipsFractional += increment; // keep this internally for partial ship generation
                star.Value.Ships = (int) star.Value.ShipsFractional;
            }

            // Position Updates
            // We don't track exact location since ships can only be
            // exactly between stars, and nothing can happen to them
            // in between
            foreach (var fleet in state.Fleets)
            {
                if (fleet.Value.InTransit)
                {
                    // Take into account warp gates
                    var speedModifier = 1;
                    if (fleet.Value.OriginStar.WarpGate == 1 && fleet.Value.DestinationStar.WarpGate == 1)
                    {
                        speedModifier = configuration.GetSettingAsInt(Configuration.ConfigurationKeys.WarpGateSpeedCoefficient);
                    }

                    fleet.Value.DistanceToDestination -= (configuration.GetSettingAsDouble(Configuration.ConfigurationKeys.FleetSpeed) * speedModifier);
                    if (fleet.Value.DistanceToDestination <= 0)
                    {
                        fleet.Value.CurrentStar = fleet.Value.DestinationStar;
                        fleet.Value.DestinationStar = null;
                        fleet.Value.OriginStar = null;
                        fleet.Value.InTransit = false;
                        fleet.Value.ToProcess = true;
                    }
                }
            }

            // Research
            var playerScience = CalculateStarByPlayer(state, (x) => x.Science);
            foreach (var player in state.Players)
            {
                player.Value.Technology[player.Value.Researching].Amount += playerScience[player.Value] * configuration.GetSettingAsInt(Configuration.ConfigurationKeys.ResearchPerScience);
                var upgraded = ConsolidateResearch(configuration, player.Value, player.Value.Researching);
                if (upgraded && player.Value.Researching == Research.Terraforming)
                {
                    foreach (var star in state.Stars.Where(x => x.Value.PlayerId == player.Value.Id))
                    {
                        star.Value.TotalResources =
                            star.Value.NaturalResources +
                            (state.Players[star.Value.PlayerId].Technology[Research.Terraforming].Level *
                             configuration.GetSettingAsInt(
                                 Configuration.ConfigurationKeys.TerraformingResourceCoefficient));
                    }
                }

                if(upgraded){
                    player.Value.Researching = player.Value.ResearchingNext;
                }

                
            }

            // Conflict
            // To simplify, during conflict all ships are the same
            // When not in motion, fleets have no ships on them
            List<int> fleetsToRemoveFromGame = new List<int>();
            foreach (var fleet in state.Fleets)
            {
                if (fleet.Value.ToProcess)
                {
                    if (fleet.Value.CurrentStar.PlayerId != -1 && fleet.Value.CurrentStar.PlayerId != fleet.Value.PlayerId)
                    {
                        var attackerWeapons =
                            state.Players[fleet.Value.PlayerId].Technology[Research.Weapons].Level;
                        var defenderWeapons =
                            state.Players[fleet.Value.CurrentStar.PlayerId].Technology[Research.Weapons].Level
                            + configuration.GetSettingAsInt(Configuration.ConfigurationKeys.DefenderBonusWeapons);
                        while (fleet.Value.Ships > 0 && fleet.Value.CurrentStar.Ships > 0)
                        {
                            // defenders go first
                            fleet.Value.Ships -= defenderWeapons;
                            if (fleet.Value.Ships > 0)
                            {
                                fleet.Value.CurrentStar.Ships -= attackerWeapons;
                            }
                        }

                        if (fleet.Value.Ships > 0)
                        {
                            // attacker wins
                            // remove defender fleets if any
                            fleetsToRemoveFromGame.AddRange(
                                state.Fleets.Where(
                                    x => x.Value.PlayerId == fleet.Value.CurrentStar.PlayerId
                                        && x.Value.CurrentStar == fleet.Value.CurrentStar
                                        ).Select(x=>x.Key));
                            fleet.Value.CurrentStar.PlayerId = fleet.Value.PlayerId;
                            fleet.Value.CurrentStar.Ships = fleet.Value.Ships;
                            fleet.Value.Ships = 0;
                            fleet.Value.CurrentStar.TotalResources =
                                fleet.Value.CurrentStar.NaturalResources +
                                (state.Players[fleet.Value.CurrentStar.PlayerId].Technology[Research.Terraforming].Level *
                                    configuration.GetSettingAsInt(
                                        Configuration.ConfigurationKeys.TerraformingResourceCoefficient));
                        }
                        else
                        {
                            // no ownership change, remove attacking fleet
                            fleetsToRemoveFromGame.Add(fleet.Key);
                        }
                        // move command
                    } else if(fleet.Value.CurrentStar.PlayerId == fleet.Value.PlayerId){
                        // ships aren't allowed to stay on fleet when not in transit in our simulation
                        fleet.Value.CurrentStar.Ships += fleet.Value.Ships;
                        fleet.Value.Ships = 0;
                    }
                    else
                    {
                        fleet.Value.CurrentStar.PlayerId = fleet.Value.PlayerId;
                        fleet.Value.CurrentStar.Ships = fleet.Value.Ships;
                        fleet.Value.Ships = 0;
                        fleet.Value.CurrentStar.TotalResources =
                            fleet.Value.CurrentStar.NaturalResources +
                            (state.Players[fleet.Value.CurrentStar.PlayerId].Technology[Research.Terraforming].Level *
                                configuration.GetSettingAsInt(
                                    Configuration.ConfigurationKeys.TerraformingResourceCoefficient));
                    }

                    fleet.Value.ToProcess = false;
                }
            }

            foreach (var fleetToRemove in fleetsToRemoveFromGame)
            {
                state.Fleets.Remove(fleetToRemove);
            }

            // * Calculate any new moves
            foreach (ISaloBot bot in bots)
            {
                bot.Run(state);
            }

            // * Sleep until tick is complete
        }
    }
}
