using Quantify.Jobs.Core.CQRS.Base;
using Quantify.Jobs.Core.Interfaces.Repositories;

namespace Quantify.Jobs.Core.CQRS.Queries.Client
{
    public class GetAllClientsQuery : IQuery<IEnumerable<Entities.Client>>
    {
        // No parameters needed for getting all clients
    }

    public class GetAllClientsQueryHandler : IQueryHandler<GetAllClientsQuery, IEnumerable<Entities.Client>>
    {
        private readonly IClientRepository _clientRepository;

        public GetAllClientsQueryHandler(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<IEnumerable<Entities.Client>> Handle(GetAllClientsQuery query, CancellationToken cancellationToken)
        {
            return await _clientRepository.GetAllAsync();
        }
    }
}