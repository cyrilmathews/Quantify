using Microsoft.Extensions.DependencyInjection;
using Quantify.Estimates.Core.CQRS.Base;

public interface IEventDispatcher
{
    Task Dispatch<TEvent>(TEvent eventInstance, CancellationToken cancellationToken) where TEvent : IEvent;
}

public class EventDispatcher : IEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public EventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Dispatch<TEvent>(TEvent eventInstance, CancellationToken cancellationToken) where TEvent : IEvent
    {
        var handler = _serviceProvider.GetRequiredService<IEventHandler<TEvent>>();
        await handler.Handle(eventInstance, cancellationToken);
    }
}