namespace Quantify.Jobs.Core.CQRS.Base
{
    public interface IQuery<TResult>
    {
    }

    public interface IQueryHandler<TCommand, TResult>
    {
        Task<TResult> Handle(TCommand command, CancellationToken cancellationToken = default);
    }
}
