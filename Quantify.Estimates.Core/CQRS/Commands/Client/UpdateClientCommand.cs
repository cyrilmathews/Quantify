using Quantify.Estimates.Core.CQRS.Base;
using Quantify.Estimates.Core.Extensions;
using Quantify.Estimates.Core.Interfaces.Repositories;

namespace Quantify.Estimates.Core.CQRS.Commands.Client
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
            var client = await _clientRepository.GetByIdAsync(command.Client.Id);

            if (client == null)
            {
                throw new InvalidOperationException($"Client with ID {command.Client.Id} does not exist.");
            }

            if (client.SourceVersion.IsRowVersionLaterThan(command.Client.SourceVersion))
            {
                throw new InvalidOperationException($"Client with ID {command.Client.Id} has a newer version.");
            }

            return await _clientRepository.UpdateAsync(command.Client);
        }
    }
}