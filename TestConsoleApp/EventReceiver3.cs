using RandomSolutions;
using System;
using System.Linq;

namespace TestConsoleApp
{
    public class EventReceiver3 : IEventBusReceiver<Event>
    {
        public Event[] Events => new[] { Event.E3 };
        public short Priority => 10;


        public void OnPublish(IEventBusArgs<Event> e)
        {
            Console.WriteLine($"{e.Event}: {e.Data.FirstOrDefault()} ({this.GetType().Name})");
            //e.Host.Publish(this, Event.E1, "retest 1");
        }
    }
}
