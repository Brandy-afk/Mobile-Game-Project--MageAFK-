using Backend.Domain.Modals;

namespace Backend.Repositories.Interfaces
{
    public interface IRecipeShopRepository
    {
        Task<RecipeShop?> GetRecipeShopByPlayerIdAsync(int playerId);
        Task<RecipeShop> CreateRecipeShopAsync(int playerId, List<int> recipeIds);
        Task DeleteRecipeShopAsync(int playerId);
    }
}
