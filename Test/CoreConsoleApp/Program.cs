using RandomSolutions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreConsoleApp
{
    class Program
    {
        //static IEventBus<Event> EventBus = new EventBus<Event>();
        static IEventBus<Event> EventBus = IoC.Container.GetInstance<IEventBus<Event>>();


        static void Main(string[] args)
        {
            EventBus.OnError += (s, e) => Console.WriteLine(e.Data);

            var token = EventBus.Subscribe(null,
                e => Console.WriteLine($"{e.Event}: {e.Data[0]}"),
                Event.E1, Event.E2);

            EventBus.Publish(null, Event.E1, "test1");
            EventBus.Unsubscribe(token);
            EventBus.Publish(null, Event.E2, "test2");


            TestSpeed();
            Console.ReadKey();
        }


        static void TestSpeed()
        {
            Task.Run(() =>
            {
                var start = DateTime.Now;
                var subsCount = 100000;

                var tokens = Enumerable.Range(0, subsCount)
                    .Select(x => EventBus.Subscribe(null, e => { /*Console.WriteLine($"{x}) {e.Event}: {e.Data[0]}");*/ }, Event.E3))
                    .ToArray();

                Console.WriteLine($"\nSubscribing: {(int)(subsCount / (DateTime.Now - start).TotalSeconds)} subs/sec");

                start = DateTime.Now;

                var eventsCount = 10;

                foreach (var x in Enumerable.Range(0, eventsCount))
                    EventBus.Publish(null, Event.E3, $"test#{x}");

                Console.WriteLine($"Publishing:  {(int)(subsCount * eventsCount / (DateTime.Now - start).TotalSeconds)} invokes/sec");

                start = DateTime.Now;

                EventBus.Unsubscribe(tokens);

                Console.WriteLine($"Unsubscribing: {(int)(subsCount / (DateTime.Now - start).TotalSeconds)} unsubs/sec");
            });
        }
    }


}
