using System;
using System.Configuration;
using Autofac;
using GreenPipes;
using MassTransit;
using MassTransit.Azure.ServiceBus.Core;
using MassTransitTestFailureNotification.Consuming;
using MassTransitTestFailureNotification.Events;
using Microsoft.Azure.ServiceBus.Primitives;

namespace MassTransitTestFailureNotification
{
    public class BusFactory
    {
        public static IMessageBus CreateBus(string serviceBusPath)
        {
            // Creates our bus from the factory
            return new MessageBus
            {
                Instance = Bus.Factory.CreateUsingAzureServiceBus(sbc =>
                {
                    ConfigureServiceBus(serviceBusPath, sbc);
                })
            };
        }

        /// <summary>
        /// Boiler plate code to configure a service bus using configuration put into the ConfigurationManager
        /// </summary>
        /// <param name="servicePath"></param>
        /// <param name="sbc"></param>
        /// <returns></returns>
        private static IServiceBusHost ConfigureServiceBus(string servicePath, IServiceBusBusFactoryConfigurator sbc)
        {
            var serviceBusConnection = ServiceBusConnectionGeneator.Generate(servicePath);

            sbc.EnablePartitioning = false;

            return sbc.Host(new Uri(serviceBusConnection.Endpoint),
                h =>
                {
                    h.TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(serviceBusConnection.SasKeyName, serviceBusConnection.SasKey, TimeSpan.FromDays(1), TokenScope.Namespace);
                });
        }

        public static IMessageBus CreateBusAndRegisterSubscriber<T>(string servicePath, string subscriptionName, IComponentContext context, TimeSpan timeToLive = default(TimeSpan)) where T : class, IEvent
        {
            return new MessageBus()
            {
                Instance = Bus.Factory.CreateUsingAzureServiceBus(
                    sbc =>
                    {
                        var host = ConfigureServiceBus(servicePath, sbc);
                        sbc.SubscriptionEndpoint<T>(host, subscriptionName, ec =>
                        {
                            ec.Consumer<EventConsumer<T>>(context);
                            if (timeToLive != default(TimeSpan))
                            {
                                ec.AutoDeleteOnIdle = timeToLive;
                            }

                            sbc.UseMessageRetry(x => x.Interval(int.Parse(ConfigurationManager.AppSettings[RetryConfigConstants.ServiceBusRetryCount]),TimeSpan.FromMilliseconds(double.Parse(ConfigurationManager.AppSettings[RetryConfigConstants.ServiceBusRetryInterval]))));
                        });
                    })
            };
        }
    }

    public static class RetryConfigConstants
    {
        public const string ServiceBusRetryCount = "ServiceBus.RetryCount";
        public const string ServiceBusRetryInterval = "ServiceBus.RetryInterval";
        public const string ServiceBusRetryIntervalIncrement = "ServiceBus.RetryIntervalIncrement";
    }
}