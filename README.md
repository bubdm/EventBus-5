# EventBus [![NuGet version](https://badge.fury.io/nu/RandomSolutions.EventBus.svg)](http://badge.fury.io/nu/RandomSolutions.EventBus)
Publisher-subscriber (pub/sub) pattern implementation

## Example
```C#
var eventbus = new EventBus<string>();
eventbus.Subscribe(null, e => Console.WriteLine($"{e.Event}: {e.Data[0]}"), "event1", "event2");
eventbus.Publish(null, "event1", "test1");
eventbus.Publish(null, "event2", "test2");

// Console output:
// event1: test1
// event2: test2
```
