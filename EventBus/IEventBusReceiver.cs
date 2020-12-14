namespace RandomSolutions
{
    public interface IEventBusReceiver<TEvent> : IEventBusReceiver
    {
        TEvent[] Events { get; }
        void OnPublish(IEventBusArgs<TEvent> args);
    }

    public interface IEventBusReceiver
    {
        short Priority { get; }
    }
}
