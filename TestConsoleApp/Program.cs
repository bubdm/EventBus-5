using Microsoft.Extensions.DependencyInjection;
using RandomSolutions;
using System;
using System.Linq;

namespace TestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var eventBus = IoC.Container.GetService<IEventBus<Event>>();

            eventBus.OnError += (s, e) => Console.WriteLine(e.Data);

            var token = eventBus.Subscribe(null,
                e => Console.WriteLine($"{e.Event}: {e.Data.FirstOrDefault()}"),
                Event.E1, Event.E2, Event.E3);

            eventBus.Publish(null, Event.E1, "test1");
            eventBus.Unsubscribe(token);
            eventBus.Publish(null, Event.E2, "test2");

            TestSpeed(eventBus);
        }

        static void TestSpeed(IEventBus<Event> eventBus)
        {
            var start = DateTime.Now;
            var subsCount = 1000000;

            var tokens = Enumerable.Range(0, subsCount)
                .Select(x => eventBus.Subscribe(null, e => { /*Console.WriteLine($"{x}) {e.Event}: {e.Data[0]}");*/ }, Event.Speed))
                .ToArray();

            Console.WriteLine($"\nSubscribing: {(int)(subsCount / (DateTime.Now - start).TotalSeconds)} subs/sec");

            start = DateTime.Now;

            var eventsCount = 10;

            foreach (var x in Enumerable.Range(0, eventsCount))
                eventBus.Publish(null, Event.Speed, $"test#{x}");

            Console.WriteLine($"Publishing:  {(int)(subsCount * eventsCount / (DateTime.Now - start).TotalSeconds)} invokes/sec");

            start = DateTime.Now;

            eventBus.Unsubscribe(tokens);

            Console.WriteLine($"Unsubscribing: {(int)(subsCount / (DateTime.Now - start).TotalSeconds)} unsubs/sec");
        }
    }


}
