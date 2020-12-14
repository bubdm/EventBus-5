using RandomSolutions;
using System;

namespace TestConsoleApp
{
    public class EventReceiver1 : IEventBusReceiver<Event>
    {
        public Event[] Events => new[] { Event.E1 };
        public short Priority => 10;


        public void OnPublish(IEventBusArgs<Event> e)
        {
            Console.WriteLine($"{this.GetType().Name} > {e.Event}: {e.Data[0]}");
        }
    }
}
