using System;
using System.Collections.Generic;
using System.Text;

namespace RandomSolutions
{
    public interface IEventBus<TEvent> : IEventBus
    {
        void Publish(object publisher, TEvent eventId, params object[] data);
        Guid Subscribe(object subscriber, Action<IEventBusArgs<TEvent>> action, params TEvent[] events);
        event EventHandler<EventBusEventArgs<EventBusException>> OnError;
    }

    public interface IEventBus
    {
        void Unsubscribe(params Guid[] tokens);
    }
}
