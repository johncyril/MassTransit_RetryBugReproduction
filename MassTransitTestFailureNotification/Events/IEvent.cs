using System;

namespace MassTransitTestFailureNotification.Events
{
    /// <summary>
    /// Interface which all Events broadcast on the service bus must implement
    /// </summary>
    public interface IEvent
    {
        DateTime RaisedAtUtc { get; }
        int SchemaVersion { get; }
        Guid? MessageId { get; }
        /// <summary>
        /// A unique identifier pertinent to each message and can be used to track the same message between publishers and subscribers
        /// This gets AutoMapped by MassTransit onto the MessageContext <see cref="IEventContext{T}"/>
        /// </summary>
        Guid? CorrelationId { get; }
    }
}