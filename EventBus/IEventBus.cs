using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RandomSolutions
{
    public interface IEventBus<TEvent> : IEventBus
    {
        Task Publish(object publisher, TEvent eventId, params object[] data);
        Task<Guid> Subscribe(object subscriber, Action<EventBusArgs<TEvent>> action, params TEvent[] events);
    }

    public interface IEventBus
    {
        Task Unsubscribe(params Guid[] tokens);
    }
}
