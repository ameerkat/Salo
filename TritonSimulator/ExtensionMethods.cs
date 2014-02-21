using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TritonSimulator.InternalModels;

namespace TritonSimulator
{
    public static class ExtensionMethods
    {
        public static void Initialize(this Dictionary<Player, int> d, Game g)
        {
            d = new Dictionary<Player,int>();
            foreach(var player in g.Players){
                d.Add(player, 0);
            }
        }
    }
}
