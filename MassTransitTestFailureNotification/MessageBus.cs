using System;
using MassTransit;

namespace MassTransitTestFailureNotification
{
    public class MessageBus : IMessageBus
    {
        /// <summary>
        /// <inheritdoc cref="IMessageBus.Instance"/>
        /// </summary>
        public IBus Instance { get; set; }
        public void Start()
        {
            Console.WriteLine("Starting Event Bus");
            ((IBusControl)Instance).Start();
        }

        public void Stop()
        {
            Console.WriteLine("Stopping Event Bus");
            ((IBusControl)Instance).StopAsync();
        }

        private void ReleaseUnmanagedResources()
        {
            Stop();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~MessageBus()
        {
            ReleaseUnmanagedResources();
        }
    }
}