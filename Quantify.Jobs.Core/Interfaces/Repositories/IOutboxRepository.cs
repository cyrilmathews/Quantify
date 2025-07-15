using Quantify.Jobs.Core.Entities;

namespace Quantify.Jobs.Core.Interfaces.Repositories
{
    public interface IOutboxRepository
    {
        Task<bool> AddAsync(Outbox outbox);
    }
}
