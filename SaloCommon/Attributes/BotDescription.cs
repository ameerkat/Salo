using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TritonSimulator
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class BotDescription : System.Attribute
    {
        private string _description;
        public BotDescription(string description)
        {
            this._description = description;
        }
    }
}
