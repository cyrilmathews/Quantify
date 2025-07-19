using Quantify.Estimates.Core.CQRS.Base;
using Quantify.Estimates.Core.Interfaces.Repositories;

namespace Quantify.Estimates.Core.CQRS.Queries.Client
{
    public class GetClientQuery : IQuery<Entities.Client>
    {
        public int ClientId { get; }

        public GetClientQuery(int clientId)
        {
            ClientId = clientId;
        }
    }

    public class GetClientQueryHandler : IQueryHandler<GetClientQuery, Entities.Client>
    {
        private readonly IClientRepository _clientRepository;

        public GetClientQueryHandler(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<Entities.Client> Handle(GetClientQuery query, CancellationToken cancellation)
        {
            return await _clientRepository.GetByIdAsync(query.ClientId);
        }
    }
}
