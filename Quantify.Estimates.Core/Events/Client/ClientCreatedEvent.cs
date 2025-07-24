using Quantify.Estimates.Core.CQRS.Base;
using Quantify.Estimates.Core.CQRS.Commands.Client;
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
        private readonly ICommandDispatcher _commandDispatcher;

        public ClientCreatedEventHandler(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        public async Task Handle(ClientCreatedEvent eventInstance, CancellationToken cancellationToken)
        {
            var client = new Entities.Client
            {
                Id = eventInstance.EntityId,
                Code = eventInstance.Code ?? string.Empty,
                Name = eventInstance.Name ?? string.Empty,
                SourceVersion = eventInstance.SourceVersion,
                ReplicatedOn = DateTime.UtcNow
            };

            await _commandDispatcher.Dispatch<CreateClientCommand, Entities.Client>(new CreateClientCommand(client),cancellationToken);
        }
    }
}
