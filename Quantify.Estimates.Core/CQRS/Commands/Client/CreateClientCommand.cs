using Quantify.Estimates.Core.CQRS.Base;
using Quantify.Estimates.Core.CQRS.Commands.Outbox;
using Quantify.Estimates.Core.Events.Client;
using Quantify.Estimates.Core.Interfaces.Repositories;
using System.Text.Json;

namespace Quantify.Estimates.Core.CQRS.Commands.Client
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
            var client = await _clientRepository.GetByIdAsync(command.Client.Id);

            if (client != null)
            {
                throw new InvalidOperationException($"Client with ID {command.Client.Id} already exists.");
            }

            var clientId = await _clientRepository.AddAsync(command.Client);

            if (client == null)
            {
                throw new InvalidOperationException($"Client with ID {clientId} not found after creation.");
            }

            return client;
        }
    }
}
