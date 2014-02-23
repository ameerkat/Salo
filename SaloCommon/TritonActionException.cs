using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TritonSimulator
{
    public class TritonActionException : Exception
    {
        public TritonActionException() : base(){}

        public TritonActionException(string message) : base(message){}
    }

    public class InsufficientShipsException : TritonActionException
    {

    }

    public class InsufficientRangeException : TritonActionException
    {

    }

    public class FleetRequiredToMoveException : TritonActionException
    {

    }
}
