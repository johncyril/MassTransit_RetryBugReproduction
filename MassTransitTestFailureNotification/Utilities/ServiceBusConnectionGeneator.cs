using System;
using System.Configuration;
using MassTransitTestFailureNotification.Constants;
using Microsoft.Azure.ServiceBus;

namespace MassTransitTestFailureNotification
{
    public class ServiceBusConnectionGeneator
    {
        public static ServiceBusConnectionStringBuilder Generate(string path)
        {
             var busNameSpace = ConfigurationManager.AppSettings[ServiceBusConstants.AzureServiceBusNamespace] ?? throw new ArgumentNullException("ConfigurationManager.AppSettings[ServiceBusConstants.AzureServiceBusNamespace]");
            var accessKeyName = ConfigurationManager.AppSettings[ServiceBusConstants.AzureServiceBusAccessKeyName] ?? throw new ArgumentNullException("ConfigurationManager.AppSettings[ServiceBusConstants.AzureServiceBusAccessKeyName]");
            var accessKeyValue = ConfigurationManager.AppSettings[ServiceBusConstants.AzureServiceBusAccessKeyValue] ?? throw new ArgumentNullException("ConfigurationManager.AppSettings[ServiceBusConstants.AzureServiceBusAccessKeyValue]");

            return new ServiceBusConnectionStringBuilder($"{busNameSpace}.{ServiceBusConstants.AzureServiceBusDomainName}", path, accessKeyName, accessKeyValue);
        }
    }
}