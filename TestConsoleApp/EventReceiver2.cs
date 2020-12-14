using RandomSolutions;
using System;

namespace TestConsoleApp
{
    public class EventReceiver2 : IEventBusReceiver<Event>
    {
        public EventReceiver2(Lazy<IEventBus<Event>> eventBus)
        {
            // test self ref
            _eventBus = eventBus;
        }

        readonly Lazy<IEventBus<Event>> _eventBus;

        public Event[] Events => new[] { Event.E2 };
        public short Priority => 0;

        public void OnPublish(IEventBusArgs<Event> e)
        {
            Console.WriteLine($"{this.GetType().Name} > {e.Event}: {e.Data[0]}");
            _eventBus.Value.Publish(this, Event.E3, "test3");
        }
    }
}
