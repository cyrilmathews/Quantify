namespace Quantify.Estimates.Core.CQRS.Base
{
    public interface IEvent
    {
    }

    public interface IEventHandler<TEvent> where TEvent : IEvent
    {
        Task Handle(TEvent eventInstance, CancellationToken cancellationToken);
    }
}
