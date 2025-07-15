namespace Quantify.Jobs.Core.Events
{
    public abstract class EventBase
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public int EntityId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public byte[]? SourceVersion { get; set; }
    }
}
