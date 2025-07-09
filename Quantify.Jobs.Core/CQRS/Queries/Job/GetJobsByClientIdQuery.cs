using Quantify.Jobs.Core.CQRS.Base;
using Quantify.Jobs.Core.Interfaces.Repositories;

namespace Quantify.Jobs.Core.CQRS.Queries.Job
{
    public class GetJobsByClientIdQuery : IQuery<IEnumerable<Entities.Job>>
    {
        public int ClientId { get; }
        public GetJobsByClientIdQuery(int clientId) => ClientId = clientId;
    }

    public class GetJobsByClientIdQueryHandler : IQueryHandler<GetJobsByClientIdQuery, IEnumerable<Entities.Job>>
    {
        private readonly IJobRepository _jobRepository;
        public GetJobsByClientIdQueryHandler(IJobRepository jobRepository) => _jobRepository = jobRepository;

        public async Task<IEnumerable<Entities.Job>> Handle(GetJobsByClientIdQuery query, CancellationToken cancellationToken)
            => await _jobRepository.GetByClientIdAsync(query.ClientId);
    }
}