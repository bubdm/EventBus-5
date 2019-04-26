using System;
using System.Collections.Generic;
using System.Text;

namespace RandomSolutions
{
    public class EventBusException : Exception
    {
        public EventBusException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
