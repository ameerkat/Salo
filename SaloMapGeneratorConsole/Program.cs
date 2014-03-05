using System;
using System.Linq;

namespace Salo.SaloMapGeneratorConsole
{
    class Program
    {
        static System.Drawing.Color[] colors = new System.Drawing.Color[]{ 
            System.Drawing.Color.Blue,
            System.Drawing.Color.Red,
            System.Drawing.Color.Green,
            System.Drawing.Color.Orange,
            System.Drawing.Color.Violet
        };

        private const int Size = 500;
        private const int MaxCircleSize = 10;

        static void Main(string[] args)
        {
            int players = 5;
            int startingStars = 6;
            int starsPerPlayer = 25;

            var display = new MapDisplay();
            display.Show();
            Console.WriteLine("Generating Random Map...");
            Console.WriteLine("Settings\n\tPlayers: {0},\n\tStarting Stars: {1},\n\tStars Per Player: {2}", players, startingStars, starsPerPlayer);

            Configuration config = new Configuration();
            config.Settings.Add(Configuration.ConfigurationKeys.TerraformingResourceCoefficient, "5");

            var map = Salo.MapGenerator.GenerateMap(players, startingStars, starsPerPlayer, config);

            Console.WriteLine("Generating Form Scale Factor...");
            // Uniform scale factor to form size
            double MaxX = map.Stars.Values.Max(star => star.X);
            double MaxY = map.Stars.Values.Max(star => star.Y);
            double scale = 1;
            if(MaxX > MaxY){
                scale = Size / MaxX;
            } else {
                scale = Size / MaxY;
            }

            //display.DrawBackground();

            Console.WriteLine("Drawing Map Preview...");
            foreach(var star in map.Stars.Values)
            {
                var x = (int)(star.X * scale);
                var y =  (int)(star.Y * scale);
                Console.WriteLine("({0}, {1})", x, y);
                
                display.DrawCircle(
                    x, 
                    y, 
                    star.PlayerId != -1 ? colors[star.PlayerId] : System.Drawing.Color.Black, 
                    (int)(MaxCircleSize * ((double)star.TotalResources / 45.0))
                );
            }

            Console.ReadKey();
        }
    }
}
