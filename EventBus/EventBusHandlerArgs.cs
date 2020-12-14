using System;

namespace RandomSolutions
{
    public class EventBusHandlerArgs<T> : EventArgs
    {
        public EventBusHandlerArgs(T data)
        {
            Data = data;
        }

        public T Data { get; protected set; }
    }
}
