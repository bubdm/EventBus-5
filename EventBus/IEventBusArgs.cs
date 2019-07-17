using System;
using System.Collections.Generic;
using System.Text;

namespace RandomSolutions
{
    public interface IEventBusArgs<TEvent> : IEventBusArgs
    {
        TEvent Event { get; }
    }

    public interface IEventBusArgs
    {
        object Publisher { get; }
        object[] Data { get; }
    }
}
