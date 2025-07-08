using Quantify.Jobs.Core.CQRS.Base;
using Quantify.Jobs.Core.Entities;
using Quantify.Jobs.Core.Interfaces.Repositories;

namespace Quantify.Jobs.Core.CQRS.Commands
{
    public class CreateClientCommand : ICommand<int>
    {
        public CreateClientCommand(Client client)
        {
            Client = client;
        }

        public Client Client { get; set; }
    }

    public class CreateClientCommandHandler : ICommandHandler<CreateClientCommand, int>
    {
        private readonly IClientRepository _clientRepository;

        public CreateClientCommandHandler(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<int> Handle(CreateClientCommand command, CancellationToken cancellationToken)
        {
            command.Client.CreatedBy = 1;
            command.Client.CreatedOn = DateTime.UtcNow;

            return await _clientRepository.AddAsync(command.Client);
        }
    }
}
