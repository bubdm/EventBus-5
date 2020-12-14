using System;

namespace RandomSolutions
{
    public class EventBusException : Exception
    {
        public EventBusException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
