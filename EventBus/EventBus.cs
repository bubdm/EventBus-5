using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomSolutions
{
    public class EventBus<TEvent> : IEventBus<TEvent>
    {
        public async Task Publish(object publisher, TEvent eventId, params object[] data)
        {
            await Task.Run(() => _publish(publisher, eventId, data));
        }

        public async Task<Guid> Subscribe(object subscriber, Action<EventBusArgs<TEvent>> action, params TEvent[] events)
        {
            return await Task.Run(() => _subscribe(subscriber, action, events));
        }

        public async Task Unsubscribe(params Guid[] tokens)
        {
            if (tokens?.Length > 0)
                await Task.Run(() => _unsubscribe(tokens));
        }



        void _publish(object publisher, TEvent eventId, params object[] data)
        {
            var tasks = new List<Task>();
            var unsubs = new List<Guid>();
            var invokeSub = new Action<Subscriber>(sub =>
            {
                if (!sub.Reference.IsAlive)
                    unsubs.Add(sub.Id);
                else
                    tasks.Add(Task.Run(() =>
                    {
                        try
                        {
                            sub.Action.Invoke(new EventBusArgs<TEvent>
                            {
                                Event = eventId,
                                Publisher = publisher,
                                Data = data,
                            });
                        }
                        catch (Exception ex) { }
                    }));
            });

            foreach (var anySub in _anyEventSubs)
                invokeSub(anySub.Value);

            Dictionary<Guid, Subscriber> eventSubs;

            if (_eventSubs.TryGetValue(eventId, out eventSubs))
                foreach (var eventSub in eventSubs)
                    invokeSub(eventSub.Value);

            if (unsubs.Count > 0)
                tasks.Add(Task.Run(() => _unsubscribe(unsubs)));

            Task.WhenAll(tasks).Wait();
        }

        Guid _subscribe(object subscriber, Action<EventBusArgs<TEvent>> action, params TEvent[] events)
        {
            var sub = new Subscriber
            {
                Id = Guid.NewGuid(),
                Reference = new WeakReference(subscriber ?? this),
                Events = events?.Length > 0 ? events : null,
                Action = action,
            };

            if (action != null && sub.Reference.IsAlive)
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
                    if (_subscribers.ContainsKey(token))
                    {
                        var sub = _subscribers[token];

                        if (sub.Events == null)
                            _anyEventSubs.Remove(sub.Id);
                        else
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
            public Action<EventBusArgs<TEvent>> Action { get; set; }
        }

        readonly Dictionary<Guid, Subscriber> _subscribers
            = new Dictionary<Guid, Subscriber>();

        readonly Dictionary<Guid, Subscriber> _anyEventSubs
            = new Dictionary<Guid, Subscriber>();

        readonly Dictionary<TEvent, Dictionary<Guid, Subscriber>> _eventSubs
            = new Dictionary<TEvent, Dictionary<Guid, Subscriber>>();

        readonly object _locker = new Object();
    }
}
