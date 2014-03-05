using System.Collections.Generic;

namespace Salo
{
    public class ActionLogger : IActionLogger
    {
        private readonly List<Action> _actions = new List<Action>();
        public List<Action> Actions { get { return _actions; } } 

        public void LogAction(Action action)
        {
            _actions.Add(action);
        }
    }
}
