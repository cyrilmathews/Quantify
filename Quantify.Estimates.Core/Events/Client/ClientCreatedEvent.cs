using Quantify.Estimates.Core.CQRS.Base;
using Quantify.Estimates.Core.Events.Base;

namespace Quantify.Estimates.Core.Events.Client
{
    public class ClientCreatedEvent : EventBase
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
    }

    public class ClientCreatedEventHandler : IEventHandler<ClientCreatedEvent>
    {
        public Task Handle(ClientCreatedEvent eventInstance, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
