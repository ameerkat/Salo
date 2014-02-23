using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TritonSimulator.InternalModels;

namespace TritonSimulator
{
    public class RandomBot : ISaloBot
    {
        protected Player _player;
        public RandomBot(){}

        public void Initialize(Player player){
            _player = player;
        }

        public void Run(Game game)
        {
            /*
             * Description
             */
        }
    }
}
