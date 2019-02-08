using Autofac;
using MassTransit;
using MassTransitTestFailureNotification.Consuming;
using MassTransitTestFailureNotification.Events;
using MassTransitTestFailureNotification.Modules;
using Moq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MassTransitTestFailureNotification
{
    public class MassTransitTests
    {
        [Fact]
        public async Task EventConsumerCallsObserverOnConsumptionFailure()
        {
            //Arrange
            var containerBuilder = new ContainerBuilder();
            var mockEmailService = new Mock<ITestEmailService>();
            containerBuilder.Register(x => mockEmailService.Object).As<ITestEmailService>();
            containerBuilder.Register(x => new List<string>()).As<IList<string>>().SingleInstance();
            containerBuilder.RegisterModule(new EventPublishingModule<ITestEvent>("TestEvent"));
            var exceptionConsumer = new EventConsumingModule<ExceptionThrowingEventConsumer, ITestEvent, TestEmailOnFaultConsumptionObserver>("TestEvent", $"TestSubscription-{Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "")}");
            containerBuilder.RegisterModule(exceptionConsumer);

            var container = containerBuilder.Build();
            var randomEvent = new TestEvent
            {
                CorrelationId = Guid.NewGuid(),
                RaisedAtUtc = new DateTime(1990, 02, 07)
            };

            var testConsumer = container.Resolve<EventConsumer<ITestEvent>>();

            //Act
            try
            {
                foreach (var bus in container.Resolve<IEnumerable<IMessageBus>>())
                {
                    bus.Start();
                }

                await container.Resolve<IEventPublisher<ITestEvent>>().PublishAsync(randomEvent, CancellationToken.None);
                Assert.ThrowsAsync<Exception>(() => testConsumer.OnEventReceived(It.IsAny<ConsumeContext<ITestEvent>>()));
            }
            finally
            {
                TestUtilities.DeleteSubscription(typeof(ITestEvent), exceptionConsumer.ServiceSubscriptionName);

                //Assert
                foreach (var bus in container.Resolve<IEnumerable<IMessageBus>>())
                {
                    bus.Stop();
                }
                mockEmailService.Verify(x => x.Send(It.IsAny<string>()), Times.Once);
            }
        }


        [Fact]
        public async Task EventConsumerCalledAsManyTimesAsRetryConfigrationPlusOne()
        {
            //Arrange
            var containerBuilder = new ContainerBuilder();
            var mockEmailService = new Mock<ITestEmailService>();
            containerBuilder.Register(x => mockEmailService.Object).As<ITestEmailService>();
            containerBuilder.Register(x => new List<string>()).As<IList<string>>().SingleInstance();
            containerBuilder.RegisterModule(new EventPublishingModule<ITestEvent>("TestEvent"));
            var exceptionConsumer = new EventConsumingModule<ExceptionThrowingEventConsumer, ITestEvent, TestEmailOnFaultConsumptionObserver>("TestEvent", $"TestSubscription-{Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "")}");
            containerBuilder.RegisterModule(exceptionConsumer);

            var container = containerBuilder.Build();
            var randomEvent = new TestEvent
            {
                CorrelationId = Guid.NewGuid(),
                RaisedAtUtc = new DateTime(1990, 02, 07)
            };

            var testConsumer = container.Resolve<EventConsumer<ITestEvent>>();

            //Act
            try
            {
                foreach (var bus in container.Resolve<IEnumerable<IMessageBus>>())
                {
                    bus.Start();
                }

                await container.Resolve<IEventPublisher<ITestEvent>>().PublishAsync(randomEvent, CancellationToken.None);
                Assert.ThrowsAsync<Exception>(() => testConsumer.OnEventReceived(It.IsAny<ConsumeContext<ITestEvent>>()));
            }
            //Assert
            finally
            {
                TestUtilities.WaitUntilConditionMetOrTimedOut(() => -1 > 0);

                foreach (var bus in container.Resolve<IEnumerable<IMessageBus>>())
                {
                    bus.Stop();
                }

                //verify number of times consumer was called.
                var callsMade = container.Resolve<IList<string>>();
                Assert.Equal(int.Parse(ConfigurationManager.AppSettings[RetryConfigConstants.ServiceBusRetryCount]) + 1, callsMade.Count);

                TestUtilities.DeleteSubscription(typeof(ITestEvent), exceptionConsumer.ServiceSubscriptionName);
            }
        }
    }
}

