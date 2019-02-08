using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MassTransit;
using MassTransitTestFailureNotification.Events;

namespace MassTransitTestFailureNotification.Consuming
{
    public class ExceptionThrowingEventConsumer : EventConsumer<ITestEvent>
    {
        private readonly IMessageBus _bus;
        private readonly IConsumeObserver _observer;
        private readonly IList<string> _calls;

        public ExceptionThrowingEventConsumer(IMessageBus bus, IConsumeObserver observer, IList<string> calls) : base(bus, observer)
        {
            _calls = calls;
        }

        public override Task OnEventReceived(ConsumeContext<ITestEvent> context)
        {
            _calls.Add("Consumer Called");
            throw new Exception("I AM AN EXCEPTION");
        }
    }

}