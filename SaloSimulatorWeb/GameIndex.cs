using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Salo;

namespace SaloSimulatorWeb
{
    public static class GameIndex
    {
        public static int GameId = 0;
        public static Dictionary<int, Salo.Models.Game> Games =
            new Dictionary<int, Salo.Models.Game>();
        public static Dictionary<Salo.Models.Game, IActionHandler> ActionHandlers =
            new Dictionary<Salo.Models.Game, IActionHandler>();
    }
}