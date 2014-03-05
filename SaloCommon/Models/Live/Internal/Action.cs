using System.Collections.Generic;

namespace Salo
{
    public class Action
    {
        public long Time { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
    }
}
