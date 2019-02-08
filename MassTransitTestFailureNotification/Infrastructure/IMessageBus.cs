using System;
using MassTransit;

namespace MassTransitTestFailureNotification
{
    /// <summary>
    /// Wrapper for IBus to prevent components referencing MassTransit libs directly.
    /// </summary>
    public interface IMessageBus : IDisposable
    {
        /// <summary>
        /// Instance of the underlying MassTransit IBus 
        /// </summary>
        IBus Instance { get; }

        void Start();

        void Stop();
    }
}