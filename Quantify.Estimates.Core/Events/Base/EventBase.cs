using Quantify.Estimates.Core.CQRS.Base;

namespace Quantify.Estimates.Core.Events.Base
{
    public abstract class EventBase : IEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public int EntityId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public byte[]? SourceVersion { get; set; }
    }
}
