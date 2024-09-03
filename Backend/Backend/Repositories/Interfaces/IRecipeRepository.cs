using Backend.Domain.Modals.Recipes;

namespace Backend.Repositories.Interfaces
{
    public interface IRecipeRepository
    {
        Task<Recipe?> FindAsync(int playerId, int typeId);
        Task<IEnumerable<Recipe>?> GetMultipleAsync(int playerId);
        Task<IEnumerable<Recipe>?> CreateAsync(int playerId, IEnumerable<int> typeIds);
        Task<IEnumerable<Recipe>?> UpdateAsync(int playerId, Dictionary<int, bool> unlockPairs);
        Task DeleteAsync(int playerId, int typeId);
    }
}
