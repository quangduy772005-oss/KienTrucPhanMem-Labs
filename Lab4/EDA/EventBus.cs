using System;
using System.Collections.Generic;
using EDA.Events;

namespace EDA
{
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Action<IEvent>>> _handlers = new();

        public void Subscribe<T>(Action<T> handler) where T : IEvent
        {
            var type = typeof(T);
            if (!_handlers.ContainsKey(type))
            {
                _handlers[type] = new List<Action<IEvent>>();
            }
            _handlers[type].Add(e => handler((T)e));
        }

        public void Publish<T>(T @event) where T : IEvent
        {
            var type = @event.GetType();
            if (_handlers.TryGetValue(type, out var handlers))
            {
                foreach (var handler in handlers)
                {
                    handler(@event);
                }
            }
        }

        public void Dispose()
        {
            _handlers.Clear();
        }
    }

    // Alias để phân biệt rõ ràng khi cấu hình giữa InMemory và RabbitMQ
    public class InMemoryEventBus : EventBus
    {
    }
}
