using System.Collections.Generic;

namespace Salo.SaloSimulatorWeb2.Models
{
    public class ActionLog
    {
        public List<KeyValuePair<long, Action>> Actions { get; set; }
    }

    public class Action
    {
        public string Name { get; set; }
        public List<KeyValuePair<string, string>> Parameters { get; set; } 
    }
}