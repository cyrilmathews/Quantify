namespace Quantify.Jobs.Core.CQRS.Base
{
    public interface ICommand<TResult>
    {
    }

    public interface ICommandHandler<TCommand, TResult>
    {
        Task<TResult> Handle(TCommand command, CancellationToken cancellationToken = default);
    }
}
