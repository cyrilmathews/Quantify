using Quantify.Estimates.Core.Entities;

namespace Quantify.Estimates.Core.Interfaces.Repositories
{
    public interface IOutboxRepository
    {
        Task<bool> AddAsync(Outbox outbox);
    }
}
