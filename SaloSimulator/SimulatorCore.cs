using System;
using System.Collections.Generic;
using System.Linq;

namespace Salo.SaloSimulator
{
    public class SimulatorCore
    {
        private static Random rnd = new Random();
        private List<IReportableActionHandler> actionHandlers { get; set; }

        /// <summary>
        /// Returns null if no winner, returns Player if game is won
        /// </summary>
        private static bool CheckForWinner(State state, Configuration configuration)
        {
            var requiredStars = (double)(configuration.GetSettingAsDouble(Configuration.ConfigurationKeys.StarsForVictoryPercent) * state.Stars.Count);
            var playerStars = CalculateStarByPlayer(state, (x) => 1);
            return playerStars.Any(x => x.Value >= requiredStars);
        }

        public static Player GetWinner(State state, Configuration configuration)
        {
            var requiredStars = (double)(configuration.GetSettingAsDouble(Configuration.ConfigurationKeys.StarsForVictoryPercent) * state.Stars.Count);
            var playerStars = CalculateStarByPlayer(state, (x) => 1);
            return playerStars.Where(x => x.Value >= requiredStars).Select(x => x.Key).FirstOrDefault();
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

        private static Dictionary<string, Technology> DefaultTechnologyDictionary()
        {
            return new Dictionary<string, Technology>()
            {
                {Research.Banking, new Technology() {Level = 1}},
                {Research.Experimentation, new Technology() {Level = 1}},
                {Research.Manufacturing, new Technology() {Level = 1}},
                {Research.Propulsion, new Technology() {Level = 1}},
                {Research.Scanning, new Technology() {Level = 1}},
                {Research.Terraforming, new Technology() {Level = 1}},
                {Research.Weapons, new Technology() {Level = 1}},
            };
        }

        private static string NewStarName(string seed)
        {
            return String.Format("Star {0}", seed);
        }

        public State Initialize(Configuration configuration, IList<ISaloBot> bots, IActionLogger logger, State map = null )
        {
            State game = new State();
            game.StartTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;

            game.Players = new Dictionary<int, Player>();
            for (int i = 0; i < bots.Count(); ++i)
            {
                var player = new Player()
                {
                    Id = i,
                    Technology = DefaultTechnologyDictionary(),
                    Researching = Research.Weapons, // defaults to weapons
                    ResearchingNext = Research.Weapons,
                    Name = String.Format("Player {0}", i),
                    Cash = configuration.GetSettingAsInt(Configuration.ConfigurationKeys.StartingCash)
                };
                game.Players.Add(i, player);

                var bot = bots[i];
                BotName botName = (BotName)Attribute.GetCustomAttribute(bot.GetType(), typeof(BotName));
                player.Name = String.Format("{0}(v{1}) #{2}", botName.Name, botName.Version, i);

                
            }

            if (map == null)
            {
                map = MapGenerator.GenerateMap(
                    bots.Count, 
                    configuration.GetSettingAsInt(Configuration.ConfigurationKeys.StartingStars), 
                    configuration.GetSettingAsInt(Configuration.ConfigurationKeys.StarsPerPlayer), 
                    configuration);
            }

            // Initialize Stars
            game.Stars = new Dictionary<int, Star>();
            game.Fleets = new Dictionary<int, Fleet>();
            foreach (var s in map.Stars)
            {
                var star = new Star(){
                    Id = s.Key,
                    PlayerId = s.Value.PlayerId,
                    Name = NewStarName(s.Key.ToString()),
                    X = s.Value.X,
                    Y = s.Value.Y,
                    Ships = s.Value.IsStartingStar ? configuration.GetSettingAsInt(Configuration.ConfigurationKeys.StartingShips) : 0,
                    Economy = s.Value.IsHomeStar ? configuration.GetSettingAsInt(Configuration.ConfigurationKeys.HomeStarEconomy) : 0,
                    Industry = s.Value.IsHomeStar ? configuration.GetSettingAsInt(Configuration.ConfigurationKeys.HomeStarIndustry) : 0,
                    Science = s.Value.IsHomeStar ? configuration.GetSettingAsInt(Configuration.ConfigurationKeys.HomeStarScience) : 0,
                    NaturalResources = s.Value.NaturalResources,
                    TotalResources = s.Value.TotalResources,
                    WarpGate = 0
                };
                game.Stars.Add(star.Id, star);

                // add a fleet to the homestar
                if (s.Value.IsHomeStar && configuration.GetSettingAsInt(Configuration.ConfigurationKeys.StartingFleets) > 0)
                {
                    var fleetId = game.GetNextId(typeof (Fleet));
                    game.Fleets.Add(fleetId, new Fleet()
                    {
                        Id = fleetId,
                        Name = star.Name + " 1",
                        OriginStar = null,
                        DestinationStar = null,
                        CurrentStar = star,
                        InTransit = false,
                        DistanceToDestination = 0,
                        Ships = 0,
                        ToProcess = false,
                        PlayerId = star.PlayerId
                    });
                }
            }

            // Initialize Fleets
            foreach (var player in game.Players)
            {
                // add starting fleets
                for (int i = 1; i < configuration.GetSettingAsInt(Configuration.ConfigurationKeys.StartingFleets); ++i)
                {
                    // get all unoccupied stars
                    KeyValuePair<int, Player> player1 = player;
                    var starCandidates = game.Stars.Where(x => x.Value.PlayerId == player1.Key && game.Fleets.All(y => y.Value.CurrentStar != x.Value));
                    // if no more stars are available to put starting fleets on
                    // note it is possible to have more than 1 fleet per star
                    // this is just an easy way to avoid putting double fleets on the starting star
                    if (!starCandidates.Any())
                        break;

                    var star = starCandidates.ToList().RandomElement();
                    var fleetId = game.GetNextId(typeof(Fleet));
                    game.Fleets.Add(fleetId, new Fleet()
                    {
                        Id = fleetId,
                        Name = star.Value.Name + " 1",
                        OriginStar = null,
                        DestinationStar = null,
                        CurrentStar = star.Value,
                        InTransit = false,
                        DistanceToDestination = 0,
                        Ships = 0,
                        ToProcess = false,
                        PlayerId = star.Value.PlayerId
                    });
                }
            }

            actionHandlers = new List<IReportableActionHandler>();
            // Initialize bots
            for (int i = 0; i < bots.Count(); ++i)
            {
                var bot = bots[i];
                var actionHandler = new ActionHandler(game, configuration, i, logger);
                actionHandlers.Add(actionHandler);
                bot.Initialize(game.Players[i], configuration, actionHandler);
            }

            return game;
        }

        public void ProcessTick(State state, Configuration configuration, IList<ISaloBot> bots)
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

            // Calculate Industry for owned stars
            // Stars produce Y*(X+5) every production # of ticks
            // X is tech level, Y is industry
            // We just calculate each tick and store partial ships
            // Round down when calculating for Combat
            foreach (var star in state.Stars.Where(x => x.Value.PlayerId != -1))
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
            for (int i = 0; i < bots.Count(); i++)
            {
                ISaloBot bot = bots[i];
                var report = state.ToReport(configuration, state.Players[i]);
                actionHandlers[i].SetReport(report);
                bot.Run(report);
            }

            // * Sleep until tick is complete
        }
    }
}
