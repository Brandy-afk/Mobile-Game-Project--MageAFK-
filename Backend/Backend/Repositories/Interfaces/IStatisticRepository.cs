using Backend.Domain.Modals.PlayerStatistics;

namespace Backend.Repositories.Interfaces
{
    public interface IStatisticRepository
    {
        Task<Statistic?> FindAsync(int playerId, int typeId);
        Task<IEnumerable<Statistic>?> GetMultipleAsync(int playerId);
        Task<IEnumerable<Statistic>?> CreateAsync(int playerId, IEnumerable<int> typeIds);
        Task<IEnumerable<Statistic>?> UpdateAsync(int playerId, Dictionary<int, float> valuePairs);
        Task DeleteAsync(int playerId, int typeId);
    }
}
