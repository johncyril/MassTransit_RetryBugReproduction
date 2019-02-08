using Autofac;
using MassTransit;
using MassTransitTestFailureNotification.Events;
using MassTransitTestFailureNotification.Publishing;

namespace MassTransitTestFailureNotification.Modules
{
    /// <summary>
    /// Implementation for registering Message Publishers in autofac
    /// We only need one instance of the IMessageBus in autofac when publishing messages
    /// </summary>
    public class EventPublishingModule<TContract> : Module where TContract : class, IEvent
    {
        private readonly string _serviceBusPath;

        /// <summary>
        /// Creates and Registers an EventPublisher<T> in autofac.
        /// </summary>
        /// <param name="serviceBusPath">The service bus path/namspace to publish events under</param>
        public EventPublishingModule(string serviceBusPath)
        {
            _serviceBusPath = serviceBusPath;
        }

        /// <summary>
        /// <inheritdoc cref="Module.Load"/>s
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EventPublisher<TContract>>().As<IEventPublisher<TContract>>().SingleInstance();
            builder.Register(context => BusFactory.CreateBus(_serviceBusPath)).As<IMessageBus>()
                .SingleInstance().IfNotRegistered(typeof(IMessageBus));
        }
    }
}