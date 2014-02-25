using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Salo.Models;

namespace Salo
{
    public interface ISaloBot
    {
        void Initialize(Player player, IActionHandler actionHandler);
        void Run(Game game);
    }
}
