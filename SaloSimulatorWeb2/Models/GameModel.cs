using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Salo.Live.Models;

namespace SaloSimulatorWeb2.Models
{
    public class Game
    {
        public Configuration Configuration { get; set; }
        public List<BotModel> Players { get; set; }
        public ApplicationUser Creator { get; set; }
        public Salo.Models.Game State { get; set; }
        public DateTime Created { get; set; }
        public DateTime Finished { get; set; }
    }
}