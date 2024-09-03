using Backend.Domain.Modals.Skills;

namespace Backend.Repositories.Interfaces
{
    public interface ISkillRepository
    {
        Task<Skill?> FindAsync(int playerId, int typeId);
        Task<IEnumerable<Skill>?> GetMultipleAsync(int playerId);
        Task<IEnumerable<Skill>?> CreateAsync(int playerId, IEnumerable<int> typeIds);
        Task<IEnumerable<Skill>?> UpdateAsync(int playerId, Dictionary<int, int> updatePairs);
        Task DeleteAsync(int playerId, int typeId);
    }

}
