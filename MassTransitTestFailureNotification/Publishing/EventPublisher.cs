using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransitTestFailureNotification.Events;

namespace MassTransitTestFailureNotification.Publishing
{
    /// <summary>
    /// <inheritdoc cref="IEventPublisher{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventPublisher<T> : IEventPublisher<T> where T : class, IEvent
    {
        private IMessageBus _bus;
        public EventPublisher(IMessageBus bus)
        {
            _bus = bus;
        }

        /// <summary>
        /// <inheritdoc cref="IEventPublisher{T}.PublishAsync"/>
        /// </summary>
        /// <param name="eventInstance"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task PublishAsync(T eventInstance, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _bus.Instance.Publish<T>(eventInstance, cancellationToken);
        }

        private void ReleaseUnmanagedResources()
        {
            _bus.Stop();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~EventPublisher()
        {
            ReleaseUnmanagedResources();
        }
    }
}