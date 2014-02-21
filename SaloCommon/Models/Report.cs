using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaloCommon.Models
{
    /// <summary>
    /// Game State
    /// </summary>
    public class Report
    {
        /// <summary>
        /// Is the current player an administrator for the game
        /// </summary>
        public bool admin { get; set; }

        /// <summary>
        /// Fleet speed in units per ?
        /// </summary>
        public double fleet_speed { get; set; }

        public Dictionary<int, Fleet> fleets { get; set; }

        public bool paused { get; set; }

        public int productions { get; set; }

        public double tick_fragment { get; set; }

        /// <summary>
        /// Time in epoch
        /// </summary>
        public long now { get; set; }

        /// <summary>
        /// Minutes per tick
        /// </summary>
        public int tick_rate { get; set; }

        /// <summary>
        /// Hours per production
        /// </summary>
        public int production_rate { get; set; }

        public Dictionary<int, Star> stars { get; set; }

        /// <summary>
        /// Stars required for victory
        /// </summary>
        public int stars_for_victory { get; set; }

        public bool game_over { get; set; }

        public bool started { get; set; }

        /// <summary>
        /// Epoch time of game start
        /// </summary>
        public long start_time { get; set; }

        /// <summary>
        /// Total number of stars in the game
        /// </summary>
        public int total_stars { get; set; }

        /// <summary>
        /// Current production cycle (incremented every production_rate hours)
        /// </summary>
        public int production_counter { get; set; }

        /// <summary>
        /// Current tick in the game (I think this is hours (tick_rate) but unsure)
        /// </summary>
        public int tick { get; set; }

        /// <summary>
        /// Cost per level of trading research
        /// </summary>
        public int trade_cost { get; set; }

        /// <summary>
        /// Name of the game
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Unique ID of the player requesting the report
        /// </summary>
        public int player_uid { get; set; }

        /// <summary>
        /// Whether or not the game is turn based
        /// </summary>
        public bool turn_based { get; set; }

        /// <summary>
        /// Unsure
        /// </summary>
        public int war { get; set; }

        public Dictionary<int, Player> players { get; set; }

        public int turn_based_time_out { get; set; }
    }
}
