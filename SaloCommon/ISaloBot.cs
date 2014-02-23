using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TritonSimulator
{
    public interface ISaloBot
    {
        void Initialize(TritonSimulator.InternalModels.Player player);
        void Run(TritonSimulator.InternalModels.Game game);
    }
}
