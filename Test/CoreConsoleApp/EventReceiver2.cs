using RandomSolutions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreConsoleApp
{
    public class EventReceiver2 : IEventBusReceiver<Event>
    {
        public Event[] Events => new[] { Event.E2 };

        public short Priority => 0;

        public void OnPublish(IEventBusArgs<Event> e)
        {
            Console.WriteLine($"{this.GetType().Name} > {e.Event}: {e.Data[0]}");
        }
    }
}
