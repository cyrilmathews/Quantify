using Quantify.Jobs.Core.CQRS.Base;
using Quantify.Jobs.Core.Interfaces.Repositories;

namespace Quantify.Jobs.Core.CQRS.Commands.Job
{
    public class DeleteJobCommand : ICommand<bool>
    {
        public DeleteJobCommand(int jobId)
        {
            JobId = jobId;
        }

        public int JobId { get; set; }
    }

    public class DeleteJobCommandHandler : ICommandHandler<DeleteJobCommand, bool>
    {
        private readonly IJobRepository _jobRepository;

        public DeleteJobCommandHandler(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<bool> Handle(DeleteJobCommand command, CancellationToken cancellationToken)
        {
            return await _jobRepository.DeleteAsync(command.JobId);
        }
    }
}