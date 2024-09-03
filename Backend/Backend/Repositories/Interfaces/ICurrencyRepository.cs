using Backend.Domain.Modals.Currency;

namespace Backend.Repositories.Interfaces
{
    public interface ICurrencyRepository
    {
        Task<Currency?> FindAsync(int playerId, int typeId);
        Task<IEnumerable<Currency>?> GetMultipleAsync(int playerId);
        Task<IEnumerable<Currency>?> CreateAsync(int playerId, IEnumerable<int> typeIds);
        Task<IEnumerable<Currency>?> UpdateAsync(int playerId, Dictionary<int, int> valuePairs);
        Task DeleteAsync(int playerId, int typeId);
    }
}
