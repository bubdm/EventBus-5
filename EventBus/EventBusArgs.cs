using System;
using System.Collections.Generic;
using System.Text;

namespace RandomSolutions
{
    public class EventBusArgs<TEvent> : EventArgs
    {
        public TEvent Event { get; set; }
        public object Publisher { get; set; }
        public object[] Data { get; set; }
    }
}
