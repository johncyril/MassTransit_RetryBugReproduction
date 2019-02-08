using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MassTransitTestFailureNotification.Events
{
    public class TestEvent : ITestEvent
    {
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime RaisedAtUtc { get; set; }
        public int SchemaVersion { get; }
        public Guid? MessageId { get; }
        public Guid? CorrelationId { get; set; }
        public string ReferenceNumber { get; set; }
        public decimal Amount { get; set; }
        public string SomeAwesomeReference { get; set; }
    }
}