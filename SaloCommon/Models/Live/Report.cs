using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace Salo.Live.Models
{
    /// <summary>
    /// Game State
    /// </summary>
    public class Report
    {
        public Report(State state, Configuration configuration, Player player)
        {
            // have to calculate visibilty of stars
            // have to merge configuration settings here
        }

        /// <summary>
        /// Is the current player an administrator for the game
        /// </summary>
        [JsonProperty("admin")]
        public bool IsAdministrator { get; set; }

        /// <summary>
        /// Fleet speed in units per tick
        /// </summary>
        [JsonProperty("fleet_speed")]
        public double FleetSpeed { get; set; }

        [JsonProperty("fleets")]
        public Dictionary<int, Fleet> Fleets { get; set; }

        [JsonProperty("paused")]
        public bool IsPaused { get; set; }

        [Browsable(false)]
        public int productions { get; set; }

        [Browsable(false)]
        public double tick_fragment { get; set; }

        /// <summary>
        /// Time in epoch
        /// </summary>
        [JsonProperty("now")]
        public long UnixTimestampNow { get; set; }

        /// <summary>
        /// Minutes per tick
        /// </summary>
        [JsonProperty("tick_rate")]
        public int TickRate { get; set; }

        /// <summary>
        /// Hours per production
        /// </summary>
        [JsonProperty("production_rate")]
        public int ProductionRate { get; set; }

        [JsonProperty("stars")]
        public Dictionary<int, Star> Stars { get; set; }

        /// <summary>
        /// Stars required for victory
        /// </summary>
        [JsonProperty("stars_for_victory")]
        public int StarsForVictory { get; set; }

        [JsonProperty("game_over")]
        public bool IsOver { get; set; }

        [JsonProperty("started")]
        public bool IsStarted { get; set; }

        /// <summary>
        /// Epoch time of game start
        /// </summary>
        [JsonProperty("start_time")]
        public long UnixTimestampStarted { get; set; }

        /// <summary>
        /// Total number of stars in the game
        /// </summary>
        [JsonProperty("total_stars")]
        public int TotalStars { get; set; }

        /// <summary>
        /// Current production cycle (incremented every production_rate hours)
        /// </summary>
        [JsonProperty("production_counter")]
        public int CurrentProductionCounter { get; set; }

        /// <summary>
        /// Current tick in the game (I think this is hours (tick_rate) but unsure)
        /// </summary>
        [JsonProperty("tick")]
        public int CurrentTick { get; set; }

        /// <summary>
        /// Cost per level of trading research
        /// </summary>
        [JsonProperty("trade_cost")]
        public int TradeCostPerLevel { get; set; }

        /// <summary>
        /// Name of the game
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Unique ID of the player requesting the report
        /// </summary>
        [JsonProperty("player_uid")]
        public int PlayerId { get; set; }

        /// <summary>
        /// Whether or not the game is turn based
        /// </summary>
        [JsonProperty("turn_based")]
        public bool IsTurnBased { get; set; }

        /// <summary>
        /// Unsure
        /// </summary>
        [Browsable(false)]
        public int war { get; set; }

        [JsonProperty("players")]
        public Dictionary<int, Player> Players { get; set; }

        [JsonProperty("turn_based_time_out")]
        public int TurnBasedTimeOut { get; set; }
    }
}
