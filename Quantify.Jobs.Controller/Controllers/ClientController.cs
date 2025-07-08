using Microsoft.AspNetCore.Mvc;
using Quantify.Jobs.Core.CQRS.Base;
using Quantify.Jobs.Core.CQRS.Commands;
using Quantify.Jobs.Core.CQRS.Queries.Client;
using Quantify.Jobs.Core.Entities;

namespace Quantify.Jobs.Controller.Controllers
{
    [ApiController]
    [Route("api/client")]
    public class ClientController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        public ClientController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Client>> GetClient(int id, CancellationToken cancellationToken)
        {
            var client = await _queryDispatcher.Dispatch<GetClientQuery, Client>(new GetClientQuery(id), cancellationToken);
            
            return (client == null) ? NotFound() : Ok(client);
        }

        [HttpPost]
        public async Task<ActionResult<Client>> AddClient([FromBody] Client client, CancellationToken cancellationToken)
        {
            // Add the client and get the new Id
            var clientId = await _commandDispatcher.Dispatch<CreateClientCommand, int>(
                new CreateClientCommand(client), cancellationToken);

            // Retrieve the created client
            var createdClient = await _queryDispatcher.Dispatch<GetClientQuery, Client>(
                new GetClientQuery(clientId), cancellationToken);

            if (createdClient == null)
                return NotFound();

            return CreatedAtAction(nameof(GetClient), new { id = createdClient.Id }, createdClient);
        }
    }
}