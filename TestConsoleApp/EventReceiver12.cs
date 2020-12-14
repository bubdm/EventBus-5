using RandomSolutions;
using System;
using System.Linq;

namespace TestConsoleApp
{
    public class EventReceiver12 : IEventBusReceiver<Event>
    {
        public Event[] Events => new[] { Event.E1, Event.E2 };
        public short Priority => 5;

        public void OnPublish(IEventBusArgs<Event> e)
        {
            Console.WriteLine($"{e.Event}: {e.Data.FirstOrDefault()} ({this.GetType().Name})");
        }
    }
}
