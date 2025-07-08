using Quantify.Jobs.Core.CQRS.Base;
using Quantify.Jobs.Core.Interfaces.Repositories;

namespace Quantify.Jobs.Core.CQRS.Commands.Client
{
    public class DeleteClientCommand : ICommand<bool>
    {
        public DeleteClientCommand(int clientId)
        {
            ClientId = clientId;
        }

        public int ClientId { get; set; }
    }

    public class DeleteClientCommandHandler : ICommandHandler<DeleteClientCommand, bool>
    {
        private readonly IClientRepository _clientRepository;

        public DeleteClientCommandHandler(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<bool> Handle(DeleteClientCommand command, CancellationToken cancellationToken)
        {
            return await _clientRepository.DeleteAsync(command.ClientId);
        }
    }
}