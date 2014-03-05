using System;
using System.Collections.Generic;

namespace Salo.SaloSimulatorWeb2.Models
{
    public class Game
    {
        public Configuration Configuration { get; set; }
        public List<BotModel> Players { get; set; }
        public ApplicationUser Creator { get; set; }
        public State InitialState { get; set; }
        public ActionLog Actions { get; set; }
        public DateTime Created { get; set; }
        public DateTime Finished { get; set; }
    }
}