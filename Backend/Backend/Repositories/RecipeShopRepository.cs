using Backend.Data;
using Backend.Domain.Modals;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.Interfaces
{
    public class RecipeShopRepository : IRecipeShopRepository
    {
        private readonly GameDbContext _context;

        public RecipeShopRepository(GameDbContext context)
        {
            _context = context;
        }

        public async Task<RecipeShop?> GetRecipeShopByPlayerIdAsync(int playerId)
        {
            return await _context.RecipeShops.FirstOrDefaultAsync(rs => rs.PlayerID == playerId);
        }

        public async Task<RecipeShop> CreateRecipeShopAsync(int playerId, List<int> recipeIds)
        {
            await DeleteRecipeShopAsync(playerId);

            var recipeShop = new RecipeShop
            {
                PlayerID = playerId,
                RecipeIDs = recipeIds
            };

            _context.RecipeShops.Add(recipeShop);
            await _context.SaveChangesAsync();

            return recipeShop;
        }

        public async Task DeleteRecipeShopAsync(int playerId)
        {
            var existingShop = await _context.RecipeShops
                .FirstOrDefaultAsync(rs => rs.PlayerID == playerId);

            if (existingShop != null)
            {
                _context.RecipeShops.Remove(existingShop);
                await _context.SaveChangesAsync();
            }
        }
    }
}
