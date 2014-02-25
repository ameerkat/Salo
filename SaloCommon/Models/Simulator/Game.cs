using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salo.Models
{
    public class Game
    {
        public int fleetId { get; set; }
        public int playerId { get; set; }
        public int starId { get; set; }
        public List<Fleet> Fleets { get; set; }
        public List<Player> Players { get; set; }
        public List<Star> Stars { get; set; }

        public int FleetSpeed { get; set; }
        // In microseconds
        public int TickRate { get; set; }
        public int ElapsedTicks { get; set; }
        // Ticks per production
        public int ProductionRate { get; set; }
        public double StarsForVictory { get; set; }
        public int TradeCost { get; set; }
        public int MoneyPerEconomy { get; set; }
        public int BaseTechRate { get; set; }
        public int WarpGateModifier { get; set; }
        public int DefenderBonus { get; set; }
        public int StartingCash { get; set; }
        public int StartingShips { get; set; }
        public int HomeStarEconomy { get; set; }
        public int HomeStarIndustry { get; set; }
        public int HomeStarScience { get; set; }
        public int StartingFleets { get; set; }
    }
}
