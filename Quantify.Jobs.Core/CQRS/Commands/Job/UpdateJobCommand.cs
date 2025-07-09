using Quantify.Jobs.Core.CQRS.Base;
using Quantify.Jobs.Core.Entities;
using Quantify.Jobs.Core.Interfaces.Repositories;

namespace Quantify.Jobs.Core.CQRS.Commands.Job
{
    public class UpdateJobCommand : ICommand<bool>
    {
        public UpdateJobCommand(Entities.Job job)
        {
            Job = job;
        }

        public Entities.Job Job { get; set; }
    }

    public class UpdateJobCommandHandler : ICommandHandler<UpdateJobCommand, bool>
    {
        private readonly IJobRepository _jobRepository;

        public UpdateJobCommandHandler(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<bool> Handle(UpdateJobCommand command, CancellationToken cancellationToken)
        {
            command.Job.UpdatedOn = DateTime.UtcNow;
            // Set UpdatedBy as needed, e.g., from context or a default value
            // command.Job.UpdatedBy = ...;
            return await _jobRepository.UpdateAsync(command.Job);
        }
    }
}