using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quantify.Jobs.Core.CQRS.Base;
using Quantify.Jobs.Core.CQRS.Commands.Job;
using Quantify.Jobs.Core.CQRS.Queries.Job;
using Quantify.Jobs.Core.Entities;

namespace Quantify.Jobs.Controller.Controllers
{
    [ApiController]
    [Route("api/job")]
    [Authorize(Policy = "RequiresJobsAllScope")]
    public class JobController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        public JobController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Job>> GetJob(int id, CancellationToken cancellationToken)
        {
            var job = await _queryDispatcher.Dispatch<GetJobQuery, Job?>(new GetJobQuery(id), cancellationToken);
            return job == null ? NotFound() : Ok(job);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Job>>> GetAllJobs(CancellationToken cancellationToken)
        {
            var jobs = await _queryDispatcher.Dispatch<GetAllJobsQuery, IEnumerable<Job>>(new GetAllJobsQuery(), cancellationToken);
            return Ok(jobs);
        }

        [HttpGet("by-client/{clientId:int}")]
        public async Task<ActionResult<IEnumerable<Job>>> GetJobsByClientId(int clientId, CancellationToken cancellationToken)
        {
            var jobs = await _queryDispatcher.Dispatch<GetJobsByClientIdQuery, IEnumerable<Job>>(new GetJobsByClientIdQuery(clientId), cancellationToken);
            return Ok(jobs);
        }

        [HttpPost]
        public async Task<ActionResult<Job>> AddJob([FromBody] Job job, CancellationToken cancellationToken)
        {
            var jobId = await _commandDispatcher.Dispatch<CreateJobCommand, int>(new CreateJobCommand(job), cancellationToken);
            var createdJob = await _queryDispatcher.Dispatch<GetJobQuery, Job?>(new GetJobQuery(jobId), cancellationToken);

            if (createdJob == null)
                return NotFound();

            return CreatedAtAction(nameof(GetJob), new { id = createdJob.Id }, createdJob);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Job>> UpdateJob(int id, [FromBody] Job job, CancellationToken cancellationToken)
        {
            if (id != job.Id)
                return BadRequest("Job ID mismatch.");

            var result = await _commandDispatcher.Dispatch<UpdateJobCommand, bool>(new UpdateJobCommand(job), cancellationToken);

            if (!result)
                return NotFound();

            var updatedJob = await _queryDispatcher.Dispatch<GetJobQuery, Job?>(new GetJobQuery(id), cancellationToken);

            if (updatedJob == null)
                return NotFound();

            return Ok(updatedJob);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteJob(int id, CancellationToken cancellationToken)
        {
            var result = await _commandDispatcher.Dispatch<DeleteJobCommand, bool>(new DeleteJobCommand(id), cancellationToken);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}