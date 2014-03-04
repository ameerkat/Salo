using System.Collections.Generic;

namespace Salo.Live.Models
{
    /// <summary>
    /// Game State
    /// </summary>
    public class State
    {
        public Dictionary<int, Player> Players { get; set; }
        public Dictionary<int, Fleet> Fleets { get; set; }
        public bool IsPaused { get; set; }
        public Dictionary<int, Star> Stars { get; set; }
        public bool IsOver { get; set; }
        public bool IsStarted { get; set; }
        public int CurrentTick { get; set; }
    }
}
