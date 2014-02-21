using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TritonSimulator.InternalModels;
using IniParser;
using IniParser.Model;
using System.IO;

namespace TritonSimulator
{
    class Program
    {
        private static StreamWriter GameOutput = new StreamWriter(Console.OpenStandardOutput());
        private static Random rnd = new Random();

        private const string GameParamsSection = "GameParams";
        private static void LoadSettings(Game game, IniData data){
            game.FleetSpeed = int.Parse(data[GameParamsSection]["FleetSpeed"]);
            game.TickRate = int.Parse(data[GameParamsSection]["FleetSpeed"]);
            game.ProductionRate = int.Parse(data[GameParamsSection]["ProductionRate"]);
            game.TradeCost = int.Parse(data[GameParamsSection]["ProductionRate"]);
            game.StarsForVictory = double.Parse(data[GameParamsSection]["StarsForVictory"]);
            game.MoneyPerEconomy = int.Parse(data[GameParamsSection]["MoneyPerEconomy"]);
            game.BaseTechRate = int.Parse(data[GameParamsSection]["BaseTechRate"]);
            game.WarpGateModifier = int.Parse(data[GameParamsSection]["WarpGateModifier"]);
            game.DefenderBonus = int.Parse(data[GameParamsSection]["DefenderBonus"]);
        }

        /// <summary>
        /// Returns null if no winner, returns Player if game is won
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        private static Player CheckForWinner(Game game)
        {
            var requiredStars = (int)(game.StarsForVictory * game.Stars.Count);
            var playerStars = CalculateByPlayer(game, (x) => 1);
            return playerStars.Where((x) => x.Value >= requiredStars).FirstOrDefault().Key;
        }

        /// <summary>
        /// Returns true if the research was upgraded a level
        /// (used to determine if should move to next research)
        /// </summary>
        /// <param name="game"></param>
        /// <param name="player"></param>
        /// <param name="tech"></param>
        /// <returns></returns>
        private static bool ConsolidateResearch(Game game, Player player, Technology.Technologies tech)
        {
            var wentUpALevel = false;
            var requiredResearch = player.Tech.Levels[tech] * game.BaseTechRate;
            while (player.Tech.Values[tech] > requiredResearch)
            {
                player.Tech.Levels[tech] += 1;
                wentUpALevel = true;
                player.Tech.Values[tech] -= requiredResearch;
                requiredResearch = player.Tech.Levels[tech] * game.BaseTechRate;
            }
            return wentUpALevel;
        }

        /// <summary>
        /// Calculates star stats by playter
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        private static Dictionary<Player, int> CalculateByPlayer(Game game, Func<Star, int> selector)
        {
            var statByPlayer = new Dictionary<Player, int>();
            statByPlayer.Initialize(game);
            foreach (var star in game.Stars)
            {
                if (star.Owner != null)
                    statByPlayer[star.Owner] += selector(star);
            }
            return statByPlayer;
        }

        static void Main(string[] args)
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
             * * ~Ai Makes New Moves!
             */

            Game game = new Game();
            FileIniDataParser parser = new FileIniDataParser();
            IniData data = parser.ReadFile("Configuration.ini");
            // Set Defaults
            while (true)
            {
                // Check win conditions
                Player winner = CheckForWinner(game);
                if (winner != null)
                {
                    GameOutput.WriteLine(String.Format("Player {0}({1}) has won.", winner.Id, winner.Name));
                }

                // Each pass of loop is one tick, loop should sleep to synch ticks
                game.ElapsedTicks += 1;

                // Calculate Production
                // MoneyPerEcon * Total Economy + 75 * Banking
                // Add research bonus from experimentation
                // TODO Check banking rate
                if (game.ElapsedTicks % game.ProductionRate == 0)
                {
                    var playerEconomy = CalculateByPlayer(game, (x) => x.Economy);
                    foreach (var player in game.Players)
                    {
                        player.Cash += (playerEconomy[player] * game.MoneyPerEconomy);
                        // Calculate banking bonus
                        player.Cash += (player.Tech.Levels[Technology.Technologies.Banking] * 75);
                        var researchToAdd = player.Tech.Levels[Technology.Technologies.Experimentation] * 72;
                        var randomResearch = (Technology.Technologies)rnd.Next(0, 7); // returns 0,1,...,6
                        player.Tech.Values[randomResearch] += researchToAdd;
                        // Resolve research levels before continuing
                        if(ConsolidateResearch(game, player, randomResearch)){
                            player.CurrentlyResearching = player.NextResearching;
                        }
                    }
                }

                // Calculate Industry
                // Stars produce Y*(X+5) every production # of ticks
                // X is tech level, Y is industry
                // We just calculate each tick and store partial ships
                // Round down when calculating for Combat
                foreach (var star in game.Stars)
                {
                    double incrementValuePerProduction = 
                        star.Industry * 
                        (star.Owner.Tech.Levels[Technology.Technologies.Manufacturing] * 5);
                    var increment = incrementValuePerProduction / (double)game.ProductionRate;
                    star.Ships += increment;
                }

                // Position Updates
                // We don't track exact location since ships can only be
                // exactly between stars, and nothing can happen to them
                // in between
                foreach (var fleet in game.Fleets)
                {
                    // Take into account warp gates
                    var speedModifier = 1;
                    if (fleet.OriginStar.WarpGate && fleet.DestinationStar.WarpGate)
                    {
                        speedModifier = game.WarpGateModifier;
                    }
                    
                    fleet.DistanceToDestination -= (game.FleetSpeed * speedModifier);
                    if (fleet.DistanceToDestination <= 0)
                    {
                        fleet.CurrentStar = fleet.DestinationStar;
                        fleet.DestinationStar = null;
                        fleet.OriginStar = null;
                        fleet.InTransit = false;
                        fleet.ToProcess = true;
                    }
                }

                // Research
                var playerScience = CalculateByPlayer(game, (x) => x.Science);
                foreach (var player in game.Players)
                {
                    player.Tech.Values[player.CurrentlyResearching] += playerScience[player];
                    if(ConsolidateResearch(game, player, player.CurrentlyResearching)){
                        player.CurrentlyResearching = player.NextResearching;
                    }
                }

                // Conflict
                // To simplify, during conflict all ships are the same
                // When not in motion, fleets have no ships on them
                List<Fleet> fleetsToRemoveFromGame = new List<Fleet>();
                foreach (var fleet in game.Fleets)
                {
                    if (fleet.ToProcess != true)
                    {
                        if (fleet.CurrentStar.Owner != null)
                        {
                            var attackerWeapons =
                                fleet.Owner.Tech.Levels[Technology.Technologies.Weapons];
                            var defenderWeapons =
                                fleet.CurrentStar.Owner.Tech.Levels[Technology.Technologies.Weapons]
                                + game.DefenderBonus;
                            while (fleet.Ships > 0 && fleet.CurrentStar.Ships > 0)
                            {
                                // defenders go first
                                fleet.Ships -= defenderWeapons;
                                if (fleet.Ships > 0)
                                {
                                    fleet.CurrentStar.Ships -= attackerWeapons;
                                }
                            }

                            if (fleet.Ships > 0)
                            {
                                // attacker wins
                                // remove defender fleets if any
                                fleetsToRemoveFromGame.AddRange(
                                    game.Fleets.Where(
                                        x => x.Owner == fleet.CurrentStar.Owner 
                                            && x.CurrentStar == fleet.CurrentStar
                                            ));
                                fleet.CurrentStar.Owner = fleet.Owner;
                                fleet.CurrentStar.Ships = fleet.Ships;
                                fleet.Ships = 0;
                            }
                            else
                            {
                                // no ownership change, remove attacking fleet
                                fleetsToRemoveFromGame.Add(fleet);
                            }
                        }
                        else
                        {
                            fleet.CurrentStar.Owner = fleet.Owner;
                            fleet.CurrentStar.Ships = fleet.Ships;
                            fleet.Ships = 0;
                        }
                        fleet.ToProcess = false;
                    }
                }

                foreach (var fleetToRemove in fleetsToRemoveFromGame)
                {
                    game.Fleets.Remove(fleetToRemove);
                }

                // * Calculate any new moves

                // * Sleep until tick is complete
            }
        }
    }
}
