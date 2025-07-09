using Quantify.Jobs.Core.CQRS.Base;
using Quantify.Jobs.Core.Interfaces.Repositories;

namespace Quantify.Jobs.Core.CQRS.Queries.Job
{
    public class GetJobQuery : IQuery<Entities.Job?>
    {
        public int JobId { get; }
        public GetJobQuery(int jobId) => JobId = jobId;
    }

    public class GetJobQueryHandler : IQueryHandler<GetJobQuery, Entities.Job?>
    {
        private readonly IJobRepository _jobRepository;
        public GetJobQueryHandler(IJobRepository jobRepository) => _jobRepository = jobRepository;

        public async Task<Entities.Job?> Handle(GetJobQuery query, CancellationToken cancellationToken)
            => await _jobRepository.GetByIdAsync(query.JobId);
    }
}