using Quantify.Estimates.Core.CQRS.Base;
using Quantify.Estimates.Core.Events.Base;
using Quantify.Estimates.Core.Interfaces.Repositories;

namespace Quantify.Estimates.Core.CQRS.Commands.Outbox
{
    public class AddOutboxEventCommand : ICommand<bool>
    {
        public AddOutboxEventCommand(EventBase eventInfo, string payload)
        {
            EventInfo = eventInfo ?? throw new ArgumentNullException(nameof(eventInfo));
            Payload = payload ?? throw new ArgumentNullException(nameof(payload));
        }

        public EventBase EventInfo { get; }
        public string Payload { get; }
    }

    public class AddOutboxEventCommandHandler : ICommandHandler<AddOutboxEventCommand, bool>
    {
        private readonly IOutboxRepository _outboxRepository;

        public AddOutboxEventCommandHandler(IOutboxRepository outboxRepository)
        {
            _outboxRepository = outboxRepository;
        }

        public async Task<bool> Handle(AddOutboxEventCommand command, CancellationToken cancellationToken)
        {
            var outbox = new Entities.Outbox
            {
                Id = command.EventInfo.EventId,
                EventType = command.EventInfo.GetType().FullName,
                EventData = command.Payload,
                Timestamp = DateTime.UtcNow,
                IsProcessed = false
            };

            return await _outboxRepository.AddAsync(outbox);
        }
    }
}
