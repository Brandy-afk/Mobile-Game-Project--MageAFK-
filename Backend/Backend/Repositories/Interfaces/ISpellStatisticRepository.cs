using Backend.Domain.Inputs.DTO;
using Backend.Domain.Modals.Spells;

namespace Backend.Repositories.Interfaces
{
    public interface ISpellStatisticRepository
    {
        Task<SpellStatistic?> FindAsync(int playerId, int typeId);
        Task<IEnumerable<SpellStatistic>?> GetMultipleAsync(int playerId);
        Task<IEnumerable<SpellStatistic>?> CreateAsync(int playerId, IEnumerable<int> typeIds);
        Task<IEnumerable<SpellStatistic>?> UpdateAsync(int playerId, Dictionary<int, SpellStatisticUpdateInput> updatePairs);
        Task DeleteAsync(int playerId, int typeId);
    }

}
