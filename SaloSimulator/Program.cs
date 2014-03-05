using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;

namespace Salo.SaloSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            const int players = 3;
            var configuration = ConfigurationHelper.LoadConfigurationFromIni("Configuration.Default.ini");

            var bots = new List<ISaloBot>();
            for(int i = 0; i < players; ++i)
            {
                var bot = new RandomBot.RandomBot();
                bots.Add(bot);
            }

            IActionLogger actionLogger = new ActionLogger();
            SimulatorCore simulatorCore = new SimulatorCore();
            var game = simulatorCore.Initialize(configuration, bots, actionLogger);
            while (!game.IsOver)
            {
                simulatorCore.ProcessTick(game, configuration, bots);
                Console.WriteLine("Tick {0}", game.CurrentTick);
            }

            var winner = SimulatorCore.GetWinner(game, configuration);
            Console.WriteLine("Game over, winner: {0}", winner.Name);
        }
    }
}
