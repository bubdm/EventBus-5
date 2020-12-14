using Microsoft.Extensions.DependencyInjection;
using RandomSolutions;
using SimpleInjector;
using System;
using System.Linq;
using System.Reflection;

namespace TestConsoleApp
{
    class IoC
    {
        public static IServiceProvider Container => _microsoftDependencyInjection();
        //public static IServiceProvider Container => _simpleInjector();

        static IServiceProvider _microsoftDependencyInjection()
        {
            var services = new ServiceCollection();

            services.AddTransient(typeof(Lazy<>), typeof(LazyService<>));

            services.AddSingleton<IEventBus<Event>, EventBus<Event>>();

            foreach (var x in Assembly.GetExecutingAssembly().GetTypes().Where(x => typeof(IEventBusReceiver<Event>).IsAssignableFrom(x)))
                services.AddSingleton(typeof(IEventBusReceiver<Event>), x);

            return services.BuildServiceProvider();
        }

        static IServiceProvider _simpleInjector()
        {
            var container = new Container();

            container.Register<IServiceProvider>(() => container, Lifestyle.Singleton);
            container.Register(typeof(Lazy<>), typeof(LazyService<>), Lifestyle.Singleton);

            container.Register<IEventBus<Event>, EventBus<Event>>(Lifestyle.Singleton);

            foreach (var x in Assembly.GetExecutingAssembly().GetTypes().Where(x => typeof(IEventBusReceiver<Event>).IsAssignableFrom(x)))
                container.Collection.Append(typeof(IEventBusReceiver<Event>), x, Lifestyle.Singleton);

            container.Verify();

            return container;
        }

        class LazyService<T> : Lazy<T>
        {
            public LazyService(IServiceProvider serviceProvider)
                : base(serviceProvider.GetRequiredService<T>)
            {
            }
        }
    }
}
