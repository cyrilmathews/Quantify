namespace Quantify.Estimates.Core.CQRS.Base
{
    public interface ICommand
    {
    }

    public interface ICommand<TResult> : ICommand
    {
    }

    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        Task Handle(TCommand command, CancellationToken cancellationToken);
    }

    public interface ICommandHandler<TCommand, TResult> where TCommand : ICommand<TResult>
    {
        Task<TResult> Handle(TCommand command, CancellationToken cancellationToken);
    }
}
