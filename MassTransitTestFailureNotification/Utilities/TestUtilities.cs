using System;
using System.Threading;
using MassTransit.Azure.ServiceBus.Core.Contexts;
using MassTransit.Azure.ServiceBus.Core.Pipeline;
using Microsoft.Azure.ServiceBus.Primitives;


namespace MassTransitTestFailureNotification
{
    public class TestUtilities
    {
        public static void DeleteSubscription(Type type, string serviceSubscriptionName)
        {
            var manager = CreateNamespaceManager();
            manager.DeleteSubscriptionAsync(type.Namespace + "/" + type.Name, serviceSubscriptionName);
        }

        public static NamespaceManager CreateNamespaceManager()
        {
            var connection = ServiceBusConnectionGeneator.Generate("");

            return new NamespaceManager(new Uri(connection.Endpoint), new NamespaceManagerSettings()
            {
                TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(connection.SasKeyName, connection.SasKey, TimeSpan.FromDays(1), TokenScope.Namespace)
            });
        }

        public static void WaitUntilConditionMetOrTimedOut(Func<bool> conditionMet)
        {
            var timeoutExpired = false;
            var startTime = DateTime.Now;
            while (!conditionMet() && !timeoutExpired)
            {
                Thread.Sleep(100);
                timeoutExpired = DateTime.Now - startTime > TimeSpan.FromSeconds(30);
            }
        }
    }
}