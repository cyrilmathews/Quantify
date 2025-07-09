using Quantify.Jobs.Core.CQRS.Base;
using Quantify.Jobs.Core.Interfaces.Repositories;

namespace Quantify.Jobs.Core.CQRS.Queries.Job
{
    public class GetAllJobsQuery : IQuery<IEnumerable<Entities.Job>>
    {
    }

    public class GetAllJobsQueryHandler : IQueryHandler<GetAllJobsQuery, IEnumerable<Entities.Job>>
    {
        private readonly IJobRepository _jobRepository;
        public GetAllJobsQueryHandler(IJobRepository jobRepository) => _jobRepository = jobRepository;

        public async Task<IEnumerable<Entities.Job>> Handle(GetAllJobsQuery query, CancellationToken cancellationToken)
            => await _jobRepository.GetAllAsync();
    }
}