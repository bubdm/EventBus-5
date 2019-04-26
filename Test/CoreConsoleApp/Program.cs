using RandomSolutions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var eventBus = new EventBus<Event>();

            var token = eventBus.Subscribe(null,
                e => Console.WriteLine($"{e.Event}: {e.Data[0]}"),
                Event.E1, Event.E2);

            //eventBus.Unsubscribe(token);
            eventBus.Publish(null, Event.E1, "test1");
            eventBus.Publish(null, Event.E2, "test2");



            Task.Run(() =>
            {
                var start = DateTime.Now;
                var subsCount = 100000;
                
                var tokens = Enumerable.Range(0, subsCount)
                    .Select(x => eventBus.Subscribe(null, e => { /*Console.WriteLine($"{x}) {e.Event}: {e.Data[0]}");*/ }, Event.E3))
                    .ToArray();

                Console.WriteLine($"\nSubscribing: {(int)(subsCount / (DateTime.Now - start).TotalSeconds)} subs/sec");

                start = DateTime.Now;

                var eventsCount = 10;

                foreach(var x in Enumerable.Range(0, eventsCount))
                    eventBus.Publish(null, Event.E3, $"test#{x}");

                Console.WriteLine($"Publishing:  {(int)(subsCount * eventsCount / (DateTime.Now - start).TotalSeconds)} invokes/sec");

                start = DateTime.Now;

                eventBus.Unsubscribe(tokens);

                Console.WriteLine($"Unsubscribing: {(int)(subsCount / (DateTime.Now - start).TotalSeconds)} unsubs/sec");
            });

            Console.ReadKey();
        }
    }

    enum Event { E1, E2, E3 }
}
