using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;

namespace Salo.SaloSimulator
{
    class Program
    {
        private static Configuration LoadConfigurationFromIni(string iniPath)
        {
            FileIniDataParser parser = new FileIniDataParser();
            IniData data = parser.ReadFile("Configuration.Default.ini");
            Configuration configuration = new Configuration();
            foreach (var section in data.Sections)
            {
                foreach (var key in section.Keys)
                {
                    configuration.Settings.Add(key.KeyName, key.Value);
                }
            }
            return configuration;
        }

        static void Main(string[] args)
        {
            const int players = 3;
            var configuration = LoadConfigurationFromIni("Configuration.Default.ini");

            var bots = new List<ISaloBot>();
            for(int i = 0; i < players; ++i)
            {
                var bot = new RandomBot.RandomBot();
                bots.Add(bot);
            }

            var game = SimulatorCore.Initialize(configuration, bots);
            while (!game.IsOver)
            {
                SimulatorCore.ProcessTick(game, configuration, bots);
            }

            var winner = SimulatorCore.GetWinner(game, configuration);
            Console.WriteLine("Game over, winner: {0}", winner.Name);
        }
    }
}
