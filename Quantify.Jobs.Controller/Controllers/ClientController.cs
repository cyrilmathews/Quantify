using Microsoft.AspNetCore.Mvc;
using Quantify.Jobs.Core.CQRS.Base;
using Quantify.Jobs.Core.CQRS.Commands.Client;
using Quantify.Jobs.Core.CQRS.Queries.Client;
using Quantify.Jobs.Core.Entities;

namespace Quantify.Jobs.Controller.Controllers
{
    [ApiController]
    [Route("api/clients")]
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
            var createdClient = await _commandDispatcher.Dispatch<CreateClientCommand, Client>(new CreateClientCommand(client), cancellationToken);

            if (createdClient == null)
                return StatusCode(500);

            return CreatedAtAction(nameof(GetClient), new { id = createdClient.Id }, createdClient);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetAllClients(CancellationToken cancellationToken)
        {
            var clients = await _queryDispatcher.Dispatch<GetAllClientsQuery, IEnumerable<Client>>(new GetAllClientsQuery(), cancellationToken);
            return Ok(clients);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Client>> UpdateClient(int id, [FromBody] Client client, CancellationToken cancellationToken)
        {
            if (id != client.Id)
            {
                return BadRequest("Client ID mismatch.");
            }

            var result = await _commandDispatcher.Dispatch<UpdateClientCommand, bool>(new UpdateClientCommand(client), cancellationToken);

            if (!result)
            {
                return NotFound();
            }

            // Fetch the updated client data
            var updatedClient = await _queryDispatcher.Dispatch<GetClientQuery, Client>(new GetClientQuery(id), cancellationToken);

            if (updatedClient == null)
            {
                return NotFound();
            }

            return Ok(updatedClient);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteClient(int id, CancellationToken cancellationToken)
        {
            var result = await _commandDispatcher.Dispatch<DeleteClientCommand, bool>(new DeleteClientCommand(id), cancellationToken);
            
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}