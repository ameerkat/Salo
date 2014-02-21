using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TritonSimulator.InternalModels
{
    public class Game
    {
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
    }
}
