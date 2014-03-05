using System;
using System.Collections.Generic;
using System.Linq;

namespace Salo
{
    /// <summary>
    /// Game State
    /// </summary>
    public class State
    {
        public State()
        {
            _ids.Add(typeof(Player), 0);
            _ids.Add(typeof(Star), 0);
            _ids.Add(typeof(Fleet), 0);
        }

        public int GetNextId(Type type)
        {
            lock (_idlock)
            {
                return _ids[type]++;
            }
        }

        public Dictionary<int, Player> Players { get; set; }
        public Dictionary<int, Fleet> Fleets { get; set; }
        public bool IsPaused { get; set; }
        public Dictionary<int, Star> Stars { get; set; }
        public bool IsOver { get; set; }
        public bool IsStarted { get; set; }
        public int CurrentTick { get; set; }

        private Dictionary<Player, int> InitializePlayerIntDict()
        {
            return Players.ToDictionary(player => player.Value, player => 0);
        }

        /// <summary>
        /// Calculates star stats by player
        /// </summary>
        private Dictionary<Player, int> CalculateStarByPlayer(Func<Star, int> selector)
        {
            var statByPlayer = InitializePlayerIntDict();
            foreach (var star in Stars.Where(star => star.Value.PlayerId != -1))
                statByPlayer[Players[star.Value.PlayerId]] += selector(star.Value);
            return statByPlayer;
        }

        public Report ToReport(Configuration configuration, Player player)
        {
            var report = new Report();
            report.CurrentProductionCounter = this.CurrentTick/
                                              configuration.GetSettingAsInt(
                                                  Configuration.ConfigurationKeys.ProductionRate);
            report.CurrentTick = this.CurrentTick;
            report.FleetSpeed = configuration.GetSettingAsDouble(Configuration.ConfigurationKeys.FleetSpeed);
            report.IsPaused = false;
            report.IsStarted = true;
            // unsure what these are for
            report.productions = -1;
            report.tick_fragment = -1;
            report.UnixTimestampNow = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            report.UnixTimestampStarted = -1;
            report.IsOver = IsOver;
            report.TickRate = configuration.GetSettingAsInt(Configuration.ConfigurationKeys.TickRate);
            report.ProductionRate = configuration.GetSettingAsInt(Configuration.ConfigurationKeys.ProductionRate);
            report.TradeCostPerLevel = configuration.GetSettingAsInt(Configuration.ConfigurationKeys.TradeCostPerLevel);
            report.Name = "";
            report.IsTurnBased = true;
            report.war = -1;
            report.IsAdministrator = false;

            report.Fleets = new Dictionary<int, Fleet>();
            var myStars = this.Stars.Where(x => x.Value.PlayerId == player.Id).Select(x => x.Value);
            foreach (var fleet in this.Fleets.Values)
            {
                // calculate if fleet is visible
                if (myStars.Any(x => Geometry.CalculateEuclideanDistance(x.X, fleet.X, x.Y, fleet.Y) <=
                                     configuration.GetSettingAsDouble(
                                         Configuration.ConfigurationKeys.BaseVisibilityRange) +
                                     (configuration.GetSettingAsDouble(
                                         Configuration.ConfigurationKeys.ScanningResearchVisibilityBonus)*
                                      player.Technology[Research.Scanning].Level)) ||
                    fleet.PlayerId == player.Id)
                {
                    report.Fleets.Add(fleet.Id, new Fleet(fleet));
                }
            }

            foreach (var star in this.Stars.Values)
            {
                // calculate if star is visible
                if (myStars.Any(x => Geometry.CalculateEuclideanDistance(x.X, star.X, x.Y, star.Y) <=
                                     configuration.GetSettingAsDouble(
                                         Configuration.ConfigurationKeys.BaseVisibilityRange) +
                                     (configuration.GetSettingAsDouble(
                                         Configuration.ConfigurationKeys.ScanningResearchVisibilityBonus)*
                                      player.Technology[Research.Scanning].Level)) ||
                    star.PlayerId == player.Id)
                {
                    report.Stars.Add(star.Id, new Star(star));
                }
                else
                {
                    // reduced visibility
                    report.Stars.Add(star.Id, new Star()
                    {
                        Economy = 0,
                        Id = star.Id,
                        Industry = 0,
                        IsVisible = false,
                        IsStartingStar = false,
                        IsHomeStar = false,
                        NaturalResources = 0,
                        TotalResources = 0,
                        Name = star.Name,
                        PlayerId = star.PlayerId,
                        X = star.X,
                        Y = star.Y,
                        Ships = 0,
                        ShipsFractional = 0
                    });
                }
            }

            var stars = CalculateStarByPlayer(x => 1);
            var econ = CalculateStarByPlayer(x => x.Economy);
            var indust = CalculateStarByPlayer(x => x.Industry);
            var sci = CalculateStarByPlayer(x => x.Science);
            var ships = CalculateStarByPlayer(x => x.Ships);
            foreach (var p in this.Players.Values)
            {
                var fleets = Fleets.Count(x => x.Value.PlayerId == p.Id);
                var technology = new Dictionary<string, Technology>();
                foreach (var t in p.Technology)
                {
                    technology.Add(t.Key, new Technology()
                    {
                        Amount = p == player ? p.Technology[t.Key].Amount : 0,
                        BaseRequiredResearch = p.Technology[t.Key].BaseRequiredResearch,
                        bv = p.Technology[t.Key].bv,
                        Level = p.Technology[t.Key].Level,
                        sv = p.Technology[t.Key].sv,
                        value = p.Technology[t.Key].value
                    });
                }

                report.Players.Add(p.Id, new Player()
                {
                    AvatarId = p.AvatarId,
                    Cash = p.Cash,
                    countdown_to_war = p.countdown_to_war,
                    HasConceded = p.HasConceded,
                    huid = p.huid,
                    Id = p.Id,
                    IsBot = p.IsBot,
                    TotalStars = stars[p],
                    Researching = p == player ? p.Researching : String.Empty,
                    ResearchingNext = p == player ? p.ResearchingNext : String.Empty,
                    TotalFleets = fleets,
                    TotalShips = ships[p],
                    TotalEconomy = econ[p],
                    TotalIndustry = indust[p],
                    TotalScience = sci[p],
                    Technology = technology,
                    IsReady = p.IsReady,
                    missed_turns = p.missed_turns,
                    Reknown = p.Reknown
                });
            }

            return report;
        }

        /*
         * For Simulator Use Only
         */
        // [ScriptIgnore]
        private readonly Dictionary<Type, int> _ids = new Dictionary<Type, int>();
        private readonly Object _idlock = new object();
    }
}
