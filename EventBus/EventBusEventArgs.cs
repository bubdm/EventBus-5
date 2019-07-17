using System;
using System.Collections.Generic;
using System.Text;

namespace RandomSolutions
{
    public class EventBusEventArgs<T> : EventArgs
    {
        public EventBusEventArgs(T data)
        {
            Data = data;
        }

        public T Data { get; protected set; }
    }
}
