using Quantify.Estimates.Core.Entities;

namespace Quantify.Estimates.Core.Interfaces.Repositories
{
    public interface IClientRepository
    {
        Task<Client?> GetByIdAsync(int id);
        Task<IEnumerable<Client>> GetAllAsync();
        Task<int> AddAsync(Client client);
        Task<bool> UpdateAsync(Client client);
    }
}
