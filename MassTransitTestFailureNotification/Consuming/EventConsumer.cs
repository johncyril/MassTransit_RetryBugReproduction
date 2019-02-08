using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using GreenPipes.Contracts;
using MassTransit;
using MassTransitTestFailureNotification.Events;

namespace MassTransitTestFailureNotification.Consuming
{
    /// <summary>
    /// An Event consumer base class, abstracting us from the MassTransit library and converting ConsumeContext to IEventContext
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class EventConsumer<T> : IConsumer<T> where T : class, IEvent
    {
        private readonly IMessageBus _bus;
        private readonly IConsumeObserver _observer;
        
        protected EventConsumer(IMessageBus bus, IConsumeObserver observer)
        {
            _bus = bus;
            _observer = observer;
            _bus.Instance.ConnectConsumeObserver(_observer);
            
        }

        public abstract Task OnEventReceived(ConsumeContext<T> context);

        public virtual async Task Consume(ConsumeContext<T> context)
        {
            await OnEventReceived(context);
        }
    }
}