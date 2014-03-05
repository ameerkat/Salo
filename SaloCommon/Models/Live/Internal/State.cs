using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Salo.Live.Models
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

        /*
         * For Simulator Use Only
         */
        // [ScriptIgnore]
        private readonly Dictionary<Type, int> _ids = new Dictionary<Type, int>();
        private readonly Object _idlock = new object();
    }
}
