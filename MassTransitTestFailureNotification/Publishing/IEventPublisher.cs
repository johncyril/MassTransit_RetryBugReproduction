using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransitTestFailureNotification.Events;

namespace MassTransitTestFailureNotification
{
    /// <summary>
    /// An Event Publisher which will broadcast a message on the bus.
    /// IMessageBus instance should be created via the <see cref="BusFactory"/>and registered in autofac.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEventPublisher<T> : IDisposable where T : IEvent
    {
        /// <summary>
        /// Asyncronously publish an Event on the MessageBus
        /// </summary>
        /// <param name="eventInstance">The the Event to publish</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task PublishAsync(T eventInstance, CancellationToken cancellationToken);
    }
}