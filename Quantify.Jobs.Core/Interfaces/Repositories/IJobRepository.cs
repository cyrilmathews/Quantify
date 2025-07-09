using Quantify.Jobs.Core.Entities;

namespace Quantify.Jobs.Core.Interfaces.Repositories
{
    public interface IJobRepository
    {
        Task<Job?> GetByIdAsync(int id);
        Task<IEnumerable<Job>> GetAllAsync();
        Task<IEnumerable<Job>> GetByClientIdAsync(int clientId);
        Task<int> AddAsync(Job job);
        Task<bool> UpdateAsync(Job job);
        Task<bool> DeleteAsync(int id);
    }
}
