using System;
using System.Configuration;
using System.Threading.Tasks;
using MassTransit;
using MassTransitTestFailureNotification.Events;

namespace MassTransitTestFailureNotification.Consuming
{
    public class TestEmailOnFaultConsumptionObserver : IConsumeObserver
    {
        private readonly ITestEmailService _emailService;

        public TestEmailOnFaultConsumptionObserver(ITestEmailService emailService)
        {
            _emailService = emailService;
        }

        public Task NotifyFault<T>(FaultContext<T> context) where T : class
        {
            _emailService.Send($"A fault ocurred with consumer of event:{context.Message.GetType().Name} {context.Exception}");
            return Task.CompletedTask;
        }

        public Task PreConsume<T>(ConsumeContext<T> context) where T : class
        {
            return  Task.CompletedTask;
        }

        public Task PostConsume<T>(ConsumeContext<T> context) where T : class
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Here is where the issue is
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception) where T : class
        {
            NotifyFault(new FaultContext<T>(context, exception));
            return Task.CompletedTask;
        }
    }
}