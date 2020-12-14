using RandomSolutions;
using System;
using System.Linq;

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
            Console.WriteLine($"{e.Event}: {e.Data.FirstOrDefault()} ({this.GetType().Name})");
            _eventBus.Value.Publish(this, Event.E3, "test3");
        }
    }
}
