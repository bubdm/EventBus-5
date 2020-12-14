namespace RandomSolutions
{
    public interface IEventBusArgs<TEvent> : IEventBusArgs
    {
        IEventBus<TEvent> Host { get; }
        TEvent Event { get; }
    }

    public interface IEventBusArgs
    {
        object Publisher { get; }
        object[] Data { get; }
    }
}
