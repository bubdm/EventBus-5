using System;
using System.Collections.Generic;
using System.Text;

namespace RandomSolutions
{
    public interface IEventBusArgs<TEvent>
    {
        TEvent Event { get; }
        object Publisher { get; }
        object[] Data { get; }
    }
}
