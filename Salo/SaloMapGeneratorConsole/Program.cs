using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaloMapGeneratorConsole
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

        private const int Size = 250;
        private const int MaxCircleSize = 10;

        [STAThread]
        static void Main(string[] args)
        {
            int players = 5;
            int startingStars = 6;
            int starsPerPlayer = 25;

            var display = new MapDisplay();
            display.Show();
            Console.WriteLine("Generating Random Map...");
            Console.WriteLine("Settings\n\tPlayers: {0},\n\tStarting Stars: {1},\n\tStars Per Player: {2}", players, startingStars, starsPerPlayer);
            var map = SaloMapGenerator.MapGenerator.GenerateMap(players, startingStars, starsPerPlayer);

            Console.WriteLine("Generating Form Scale Factor...");
            // Uniform scale factor to form size
            double MaxX = map.stars.Max(star => star.x);
            double MaxY = map.stars.Max(star => star.y);
            double scale = 1;
            if(MaxX > MaxY){
                scale = Size / MaxX;
            } else {
                scale = Size / MaxY;
            }

            //display.DrawBackground();

            Console.WriteLine("Drawing Map Preview...");
            foreach(var star in map.stars)
            {
                var x = (int)(star.x * scale);
                var y =  (int)(star.y * scale);
                Console.WriteLine("({0}, {1})", x, y);
                display.DrawCircle(x, y, colors[star.player], (int)(MaxCircleSize * ((double)star.resources / 45.0)));
            }

            Console.ReadKey();
        }
    }
}
