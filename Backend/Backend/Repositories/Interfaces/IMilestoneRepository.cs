using Backend.Domain.Modals.Milestones;

namespace Backend.Repositories.Interfaces
{
    public interface IMilestoneRepository
    {
        Task<Milestone?> FindAsync(int playerId, int typeId);
        Task<IEnumerable<Milestone>?> GetMultipleAsync(int playerId);
        Task<IEnumerable<Milestone>?> CreateAsync(int playerId, IEnumerable<int> typeIds);
        Task<IEnumerable<Milestone>?> UpdateAsync(int playerId, Dictionary<int, (float Value, int Rank, int RewardPoolSize)> updatePairs);
        Task DeleteAsync(int playerId, int typeId);
    }

}
