namespace RandomSolutions
{
    public interface IEventBusArgs<TEvent> : IEventBusArgs
    {
        TEvent Event { get; }
    }

    public interface IEventBusArgs
    {
        object Publisher { get; }
        object[] Data { get; }
    }
}
