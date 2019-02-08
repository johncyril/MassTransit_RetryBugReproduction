namespace MassTransitTestFailureNotification.Events
{
    public interface ITestEvent : IEvent
    {
        string ReferenceNumber { get; set; }
        decimal Amount { get; set; }
        string SomeAwesomeReference { get; set; }
    }
}