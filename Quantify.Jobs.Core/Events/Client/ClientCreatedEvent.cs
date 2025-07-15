namespace Quantify.Jobs.Core.Events.Client
{
    public class ClientCreatedEvent : EventBase
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
    }
}
