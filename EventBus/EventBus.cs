using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomSolutions
{
    public class EventBus<TEvent> : IEventBus<TEvent>
    {
        public EventBus(IEnumerable<IEventBusReceiver<TEvent>> eventReceivers = null)
        {
            _eventReceivers = eventReceivers;
        }

        readonly IEnumerable<IEventBusReceiver<TEvent>> _eventReceivers;

        public void Publish(object publisher, TEvent eventId, params object[] data)
        {
            if (!_init) _initReceivers();

            _publish(new EventBusArgs
            {
                Host = this,
                Publisher = publisher,
                Event = eventId,
                Data = data,
            });
        }

        public Guid Subscribe(object subscriber, Action<IEventBusArgs<TEvent>> action, params TEvent[] events)
        {
            return _subscribe(new Subscriber
            {
                Id = Guid.NewGuid(),
                Reference = new WeakReference(subscriber ?? this),
                Events = events?.Length > 0 ? events : null,
                Action = action,
            });
        }

        public void Unsubscribe(params Guid[] tokens)
        {
            if (tokens?.Length > 0)
                _unsubscribe(tokens);
        }

        public event EventHandler<EventBusHandlerArgs<EventBusException>> OnError;

        void _initReceivers()
        {
            lock (_locker)
            {
                if (_init) return;

                var tokens = _eventReceivers?.GroupBy(x => x.GetType())
                    .Select(g => g.First())
                    .OrderByDescending(x => x.Priority)
                    .Select(x => Subscribe(this, e => x.OnPublish(e), x.Events))
                    .ToList();

                _init = true;
            }
        }

        void _publish(IEventBusArgs<TEvent> args)
        {
            var unsubs = new List<Guid>();

            var invokeSub = new Action<Subscriber>(sub =>
            {
                if (!sub.Reference.IsAlive)
                {
                    unsubs.Add(sub.Id);
                    return;
                }

                try
                {
                    sub.Action.Invoke(args);
                }
                catch (Exception ex)
                {
                    OnError?.Invoke(this, new EventBusHandlerArgs<EventBusException>(new EventBusException(_errorSubscriberInvoke, ex)));
                }
            });

            lock (_locker)
            {
                foreach (var anySub in _anyEventSubs)
                    invokeSub(anySub.Value);

                if (_eventSubs.ContainsKey(args.Event))
                    foreach (var eventSub in _eventSubs[args.Event])
                        invokeSub(eventSub.Value);
            }

            if (unsubs.Count > 0)
                _unsubscribe(unsubs);
        }

        Guid _subscribe(Subscriber sub)
        {
            if (sub.Action != null && sub.Reference.IsAlive)
                lock (_locker)
                {
                    _subscribers.Add(sub.Id, sub);

                    if (sub.Events == null)
                        _anyEventSubs.Add(sub.Id, sub);
                    else
                        foreach (var e in sub.Events)
                            if (!_eventSubs.ContainsKey(e))
                                _eventSubs.Add(e, new Dictionary<Guid, Subscriber>() { { sub.Id, sub } });
                            else
                                _eventSubs[e].Add(sub.Id, sub);
                }

            return sub.Id;
        }

        void _unsubscribe(IEnumerable<Guid> tokens)
        {
            lock (_locker)
                foreach (var token in tokens)
                {
                    if (!_subscribers.ContainsKey(token))
                        continue;

                    var sub = _subscribers[token];

                    if (sub.Events == null)
                    {
                        _anyEventSubs.Remove(sub.Id);
                        continue;
                    }

                    foreach (var e in sub.Events)
                    {
                        var eventSub = _eventSubs[e];
                        eventSub.Remove(sub.Id);

                        if (eventSub.Count == 0)
                            _eventSubs.Remove(e);
                    }
                }
        }

        class Subscriber
        {
            public Guid Id { get; set; }
            public WeakReference Reference { get; set; }
            public TEvent[] Events { get; set; }
            public Action<IEventBusArgs<TEvent>> Action { get; set; }
        }

        class EventBusArgs : IEventBusArgs<TEvent>
        {
            public IEventBus<TEvent> Host { get; set; }
            public TEvent Event { get; set; }
            public object Publisher { get; set; }
            public object[] Data { get; set; }
        }

        bool _init = false;

        readonly Dictionary<Guid, Subscriber> _subscribers
            = new Dictionary<Guid, Subscriber>();

        readonly Dictionary<Guid, Subscriber> _anyEventSubs
            = new Dictionary<Guid, Subscriber>();

        readonly Dictionary<TEvent, Dictionary<Guid, Subscriber>> _eventSubs
            = new Dictionary<TEvent, Dictionary<Guid, Subscriber>>();

        readonly object _locker = new object();

        static readonly string _errorSubscriberInvoke = "Subscriber's action invoke error";
    }
}
