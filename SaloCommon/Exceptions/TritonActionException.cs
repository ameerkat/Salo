using System;

namespace Salo
{
    public class TritonActionException : Exception
    {
        public TritonActionException() : base(){}

        public TritonActionException(string message) : base(message){}
    }

    public class InsufficientPlayerPermissionsException : TritonActionException
    {
        
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
