using RandomSolutions;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CoreConsoleApp
{
    class IoC
    {

        static Container _container;

        public static Container Container {
            get {
                if (_container == null)
                {
                    var container = new Container();

                    Assembly.GetExecutingAssembly().GetTypes()
                        .Where(x => typeof(IEventBusReceiver<Event>).IsAssignableFrom(x)).ToList()
                        .ForEach(x => container.Collection.Append(typeof(IEventBusReceiver<Event>), x, Lifestyle.Singleton));

                    container.Register<IEventBus<Event>, EventBus<Event>>(Lifestyle.Singleton);
                    
                    container.Verify();
                    _container = container;
                }
                return _container;
            }
        }
    }
}
