using Quantify.Jobs.Core.CQRS.Base;
using Quantify.Jobs.Core.CQRS.Commands.Outbox;
using Quantify.Jobs.Core.Events.Client;
using Quantify.Jobs.Core.Interfaces.Repositories;
using System.Text.Json;

namespace Quantify.Jobs.Core.CQRS.Commands.Client
{
    public class CreateClientCommand : ICommand<Entities.Client>
    {
        public CreateClientCommand(Entities.Client client)
        {
            Client = client;
        }

        public Entities.Client Client { get; set; }
    }

    public class CreateClientCommandHandler : ICommandHandler<CreateClientCommand, Entities.Client>
    {
        private readonly IClientRepository _clientRepository;
        private readonly ICommandDispatcher _commandDispatcher;

        public CreateClientCommandHandler(IClientRepository clientRepository, ICommandDispatcher commandDispatcher)
        {
            _clientRepository = clientRepository;
            _commandDispatcher = commandDispatcher;
        }

        public async Task<Entities.Client> Handle(CreateClientCommand command, CancellationToken cancellationToken)
        {
            using var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeAsyncFlowOption.Enabled);

            var clientId = await _clientRepository.AddAsync(command.Client);

            var client = await _clientRepository.GetByIdAsync(clientId);

            if (client == null)
            {
                throw new InvalidOperationException($"Client with ID {clientId} not found after creation.");
            }

            var clientCreatedEvent = new ClientCreatedEvent
            {
                Code = client.Code,
                Name = client.Name,
                EntityId = client.Id,
                SourceVersion = client.Version
            };

            await _commandDispatcher.Dispatch<AddOutboxEventCommand, bool>(new AddOutboxEventCommand(clientCreatedEvent, JsonSerializer.Serialize(clientCreatedEvent)), cancellationToken);

            scope.Complete();

            return client;
        }
    }
}
