using Autofac;
using Autofac.Core;
using MassTransit;
using MassTransitTestFailureNotification.Events;

namespace MassTransitTestFailureNotification.Consuming
{
    /// <summary>
    /// Implementation for registering EventConsumers in autofac.
    /// We create an instance of IMessageBus per IEvent we wish to subscribe to.
    /// This is because MassTransit only allows us to register consumers when configuring the IBus object.
    /// </summary>
    public class EventConsumingModule<TConsumer, TContract, TObserver> : Module where TContract : class, IEvent where TObserver : IConsumeObserver
    {
        public readonly string ServiceBusPath;
        public readonly string ServiceSubscriptionName;

        /// <summary>
        /// Constuctor for registering an Event Consumer on the service bus.
        /// <see cref="subscriptionName"/> must be unique across subscribers, but must be shared for instances of the same service if load balancing consumption
        /// </summary>
        /// <param name="serviceBusPath">The path/namespace on the service bus to register the subscriber</param>
        /// <param name="subscriptionName">The queue or subscription name for your service to subscribe to this event.</param>
        public EventConsumingModule(string serviceBusPath, string subscriptionName)
        {
            ServiceBusPath = serviceBusPath;
            ServiceSubscriptionName = subscriptionName;
        }

        /// <summary>
        /// Only register a consumer of TEvent if one is not present in the container.
        /// Additionally, only register (in the container) an IMessageBusInstance [to which this consumer will be registered as a subscriber] IFF the TCsonsumer was registered.
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TObserver>().As<IConsumeObserver>().InstancePerLifetimeScope();

            builder.RegisterType<TConsumer>().As(typeof(EventConsumer<TContract>)).As<TConsumer>()
                .IfNotRegistered(typeof(EventConsumer<TContract>)).InstancePerLifetimeScope();

            builder.Register(context =>
                BusFactory.CreateBusAndRegisterSubscriber<TContract>(ServiceBusPath,
                    ServiceSubscriptionName, context)).As<IMessageBus>().OnlyIf(x =>
                x.IsRegistered(new TypedService(typeof(TConsumer)))).SingleInstance();
        }
    }
}