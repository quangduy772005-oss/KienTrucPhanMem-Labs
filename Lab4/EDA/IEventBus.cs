using System;
using EDA.Events;

namespace EDA
{
    public interface IEventBus : IDisposable
    {
        void Publish<T>(T @event) where T : IEvent;
        void Subscribe<T>(Action<T> handler) where T : IEvent;
    }
}
