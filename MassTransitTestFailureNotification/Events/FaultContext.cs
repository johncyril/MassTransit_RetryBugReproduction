using System;
using MassTransit;

namespace MassTransitTestFailureNotification.Events
{
    public class FaultContext<T> where T : class
    {
        public FaultContext(ConsumeContext<T> context, Exception e)
        {
            this.CorrelationId = context.CorrelationId;
            this.FaultedMessageId = context.MessageId;
            this.MachineName = context.Host.MachineName;
            this.ProcessName = context.Host.ProcessName;
            this.ProcessId = context.Host.ProcessId;
            this.Exception = e;
            this.SourceAddress = context.SourceAddress;
            this.DestinationAddress = context.DestinationAddress;
            this.Message = context.Message;
        }

        public T Message { get; set; }

        public Uri DestinationAddress { get; set; }

        public Uri SourceAddress { get; set; }

        public Exception Exception { get; set; }

        public int ProcessId { get; set; }

        public string ProcessName { get; set; }

        public string MachineName { get; set; }

        public Guid? FaultedMessageId { get; set; }

        public Guid? CorrelationId { get; set; }
    }
}