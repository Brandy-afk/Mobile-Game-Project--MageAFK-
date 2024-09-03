using Location = Backend.Domain.Modals.Location.Location;

namespace Backend.Repositories.Interfaces
{
    public interface ILocationRepository
    {
        Task<Location?> FindAsync(int playerId, int typeId);
        Task<IEnumerable<Location>?> GetMultipleAsync(int playerId);
        Task<IEnumerable<Location>?> CreateAsync(int playerId, IEnumerable<int> typeIds);
        Task<IEnumerable<Location>?> UpdateAsync(int playerId, Dictionary<int, int> bestWavePairs);
        Task DeleteAsync(int playerId, int typeId);
    }
}
