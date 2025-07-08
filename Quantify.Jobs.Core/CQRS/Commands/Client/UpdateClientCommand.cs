using Quantify.Jobs.Core.CQRS.Base;
using Quantify.Jobs.Core.Interfaces.Repositories;

namespace Quantify.Jobs.Core.CQRS.Commands.Client
{
    public class UpdateClientCommand : ICommand<bool>
    {
        public UpdateClientCommand(Entities.Client client)
        {
            Client = client;
        }

        public Entities.Client Client { get; set; }
    }

    public class UpdateClientCommandHandler : ICommandHandler<UpdateClientCommand, bool>
    {
        private readonly IClientRepository _clientRepository;

        public UpdateClientCommandHandler(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<bool> Handle(UpdateClientCommand command, CancellationToken cancellationToken)
        {
            command.Client.UpdatedOn = DateTime.UtcNow;
            // Set UpdatedBy as needed, e.g., from context or a default value
            // command.Client.UpdatedBy = ...;
            return await _clientRepository.UpdateAsync(command.Client);
        }
    }
}