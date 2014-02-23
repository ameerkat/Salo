using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TritonSimulator
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class BotName : System.Attribute
    {
        private string _name;
        private double _version;

        public string Name { get { return _name; } }
        public double Version { get { return _version; } }
        public BotName(string name, double version)
        {
            this._name = name;
            this._version = version;
        }
    }
}
