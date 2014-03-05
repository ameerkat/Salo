using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IniParser;
using IniParser.Model;
using System.IO;

namespace Salo
{
    class Program
    {
        private static StreamWriter GameOutput = new StreamWriter(Console.OpenStandardOutput());
        private static Random rnd = new Random();

        private const string GameParamsSection = "GameParams";
        private const string MapSection = "Map";
        
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
        private static bool ConsolidateResearch(Game game, Player player, Technologies tech)
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

        private static Dictionary<Player, int> InitializePlayerIntDict(Game g)
        {
            var d = new Dictionary<Player, int>();
            foreach (var player in g.Players)
            {
                d.Add(player, 0);
            }
            return d;
        }

        /// <summary>
        /// Calculates star stats by playter
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        private static Dictionary<Player, int> CalculateByPlayer(Game game, Func<Star, int> selector)
        {
            var statByPlayer = InitializePlayerIntDict(game);
            foreach (var star in game.Stars)
            {
                if (star.Owner != null)
                    statByPlayer[star.Owner] += selector(star);
            }
            return statByPlayer;
        }

        private static string NewStarName(string seed)
        {
            return String.Format("Star {0}", seed);
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
             * * Bots Makes New Moves
             */

            Game game = new Game();
            FileIniDataParser parser = new FileIniDataParser();
            IniData data = parser.ReadFile("Configuration.ini");
            LoadSettings(game, data);
            
            // Generate Map
            var players = int.Parse(data[MapSection]["Players"]);
            var startingStars = int.Parse(data[MapSection]["StartingStars"]);
            var starsPerPlayer = int.Parse(data[MapSection]["StarsPerPlayer"]);
            var map = SaloMapGenerator.MapGenerator.GenerateMap(players, startingStars, starsPerPlayer);

            var bots = new List<ISaloBot>();

            // Initialize Players
            game.Players = new List<Player>(players);
            for (int i = 0; i < players; ++i)
            {
                var player = new Player()
                {
                    Id = i,
                    Tech = new Technology(),
                    CurrentlyResearching = Technologies.Weapons, // defaults to weapons
                    NextResearching = Technologies.Weapons,
                    Name = String.Format("Player {0}", i),
                    Cash = game.StartingCash
                };
                game.Players.Add(player);
                var bot = new RandomBot.RandomBot();
                BotName botName = (BotName) Attribute.GetCustomAttribute(typeof(RandomBot.RandomBot), typeof(BotName));
                player.Name = String.Format("{0}(v{1}) #{2}", botName.Name, botName.Version, i);
                bot.Initialize(player, new ActionHandler());
                bots.Add(bot);
            }

            // Initialize Stars
            game.Stars = new List<Star>(players * starsPerPlayer);
            game.Fleets = new List<Fleet>(players * game.StartingFleets);
            foreach (var mapStar in map.stars)
            {
                var star = new Star(){
                    Id = game.starId,
                    Owner = game.Players[mapStar.player],
                    Name = NewStarName(game.starId.ToString()),
                    x = mapStar.x,
                    y = mapStar.y,
                    Ships = mapStar.isStartingStar ? game.StartingShips : 0,
                    Economy = mapStar.isHomeStar ? game.HomeStarEconomy : 0,
                    Industry = mapStar.isHomeStar ? game.HomeStarEconomy : 0,
                    Science = mapStar.isHomeStar ? game.HomeStarScience : 0,
                    Resources = mapStar.resources,
                    WarpGate = false
                };
                game.starId++;
                game.Stars.Add(star);

                // add a fleet to the homestar
                if (mapStar.isHomeStar && game.StartingFleets > 0)
                {
                    game.Fleets.Add(new Fleet()
                    {
                        Id = game.fleetId,
                        Name = star.Name + " 1",
                        OriginStar = null,
                        DestinationStar = null,
                        CurrentStar = star,
                        InTransit = false,
                        DistanceToDestination = 0,
                        Ships = 0,
                        ToProcess = false,
                        Owner = star.Owner
                    });

                    game.fleetId++;
                }
            };

            // Initialize Fleets
            foreach (var player in game.Players)
            {
                // add starting fleets
                for (int i = 1; i < game.StartingFleets; ++i)
                {
                    // get all unoccupied stars
                    var starCandidates = game.Stars.Where(x => x.Owner == player && !game.Fleets.Any(y => y.CurrentStar == x));
                    // if no more stars are available to put starting fleets on
                    // note it is possible to have more than 1 fleet per star
                    // this is just an easy way to avoid putting double fleets on the starting star
                    if (!starCandidates.Any())
                        break;
                    var star = starCandidates.ToList().RandomElement();
                    game.Fleets.Add(new Fleet()
                    {
                        Id = game.fleetId,
                        Name = star.Name + " 1",
                        OriginStar = null,
                        DestinationStar = null,
                        CurrentStar = star,
                        InTransit = false,
                        DistanceToDestination = 0,
                        Ships = 0,
                        ToProcess = false,
                        Owner = star.Owner
                    });

                    game.fleetId++;
                }
            }

            while (true)
            {
                // Check win conditions
                Player winner = CheckForWinner(game);
                if (winner != null)
                {
                    GameOutput.WriteLine(String.Format("\nPlayer {0} ({1}) has won.", winner.Id, winner.Name));
                    GameOutput.WriteLine(String.Format("Press any key to exit..."));
                    GameOutput.Flush();
                    Console.ReadKey();
                    break;
                }

                // Each pass of loop is one tick, loop should sleep to synch ticks
                game.ElapsedTicks += 1;
                GameOutput.WriteLine(String.Format("Current Tick {0}", game.ElapsedTicks));

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
                        player.Cash += (player.Tech.Levels[Technologies.Banking] * 75);
                        var researchToAdd = player.Tech.Levels[Technologies.Experimentation] * 72;
                        var randomResearch = (Technologies)rnd.Next(0, 7); // returns 0,1,...,6
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
                        (star.Owner.Tech.Levels[Technologies.Manufacturing] * 5);
                    var increment = incrementValuePerProduction / (double)game.ProductionRate;
                    star.Ships += increment;
                }

                // Position Updates
                // We don't track exact location since ships can only be
                // exactly between stars, and nothing can happen to them
                // in between
                foreach (var fleet in game.Fleets)
                {
                    if (fleet.InTransit)
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
                    if (fleet.ToProcess)
                    {
                        if (fleet.CurrentStar.Owner != null && fleet.CurrentStar.Owner != fleet.Owner)
                        {
                            var attackerWeapons =
                                fleet.Owner.Tech.Levels[Technologies.Weapons];
                            var defenderWeapons =
                                fleet.CurrentStar.Owner.Tech.Levels[Technologies.Weapons]
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
                        } else if(fleet.CurrentStar.Owner == fleet.Owner){
                            fleet.CurrentStar.Ships += fleet.Ships;
                            fleet.Ships = 0;
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
                foreach (var bot in bots)
                {
                    bot.Run(game);
                }

                // * Sleep until tick is complete
            }
        }
    }
}
