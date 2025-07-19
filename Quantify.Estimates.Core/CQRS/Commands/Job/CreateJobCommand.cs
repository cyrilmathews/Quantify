using Quantify.Estimates.Core.CQRS.Base;
using Quantify.Estimates.Core.Entities;
using Quantify.Estimates.Core.Interfaces.Repositories;

namespace Quantify.Estimates.Core.CQRS.Commands.Job
{
    public class CreateJobCommand : ICommand<int>
    {
        public CreateJobCommand(Entities.Job job)
        {
            Job = job;
        }

        public Entities.Job Job { get; set; }
    }

    public class CreateJobCommandHandler : ICommandHandler<CreateJobCommand, int>
    {
        private readonly IJobRepository _jobRepository;

        public CreateJobCommandHandler(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<int> Handle(CreateJobCommand command, CancellationToken cancellationToken)
        {
            command.Job.CreatedBy = 1;
            command.Job.CreatedOn = DateTime.UtcNow;
            return await _jobRepository.AddAsync(command.Job);
        }
    }
}