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
                Event.E1, Event.E2).Result;

            //eventBus.Unsubscribe(token).Wait();
            eventBus.Publish(null, Event.E1, "test1");
            eventBus.Publish(null, Event.E2, "test2");



            Task.Run(() =>
            {
                var start = DateTime.Now;
                var subsCount = 10000;

                var tokens = Task.WhenAll(Enumerable.Range(0, subsCount)
                    .Select(x => eventBus.Subscribe(null, e => { /*Console.WriteLine($"{x}) {e.Event}: {e.Data[0]}");*/ }, Event.E3)))
                    .Result;

                Console.WriteLine($"\nSubscribing: {(subsCount / (DateTime.Now - start).TotalSeconds).ToString("#.##")} subs/sec");

                start = DateTime.Now;

                var eventsCount = 4;
                Task.WhenAll(Enumerable.Range(0, eventsCount)
                    .Select(x => eventBus.Publish(null, Event.E3, $"test#{x}")))
                    .Wait();

                Console.WriteLine($"Publishing:  {(subsCount * eventsCount / (DateTime.Now - start).TotalSeconds).ToString("#.##")} invokes/sec");

                start = DateTime.Now;

                eventBus.Unsubscribe(tokens).Wait();

                Console.WriteLine($"Unsubscribing: {(subsCount / (DateTime.Now - start).TotalSeconds).ToString("#.##")} unsubs/sec");
            });

            Console.ReadKey();
        }
    }

    enum Event { E1, E2, E3 }
}
