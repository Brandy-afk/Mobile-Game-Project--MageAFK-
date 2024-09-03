using Backend.Data;
using Backend.Domain.Modals.Recipes;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly GameDbContext _context;

        public RecipeRepository(GameDbContext context)
        {
            _context = context;
        }

        public async Task<Recipe?> FindAsync(int playerId, int typeId)
        {
            return await _context.Recipes
                .FirstOrDefaultAsync(r => r.PlayerID == playerId && r.TypeID == typeId);
        }

        public async Task<IEnumerable<Recipe>?> GetMultipleAsync(int playerId)
        {
            return await _context.Recipes
                .Where(r => r.PlayerID == playerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Recipe>?> CreateAsync(int playerId, IEnumerable<int> typeIds)
        {
            var newRecipes = typeIds.Select(typeId => new Recipe
            {
                PlayerID = playerId,
                TypeID = typeId,
                Unlocked = false
            }).ToList();

            await _context.Recipes.AddRangeAsync(newRecipes);
            await _context.SaveChangesAsync();

            return newRecipes;
        }

        public async Task<IEnumerable<Recipe>?> UpdateAsync(int playerId, Dictionary<int, bool> unlockPairs)
        {
            var updatedRecipes = new List<Recipe>();
            foreach (var (typeId, unlocked) in unlockPairs)
            {
                var recipe = await FindAsync(playerId, typeId);
                if (recipe != null)
                {
                    recipe.Unlocked = unlocked;
                    updatedRecipes.Add(recipe);
                }
                else
                {
                    throw new Exception("Row not found!");
                }
            }

            await _context.SaveChangesAsync();
            return updatedRecipes;
        }

        public async Task DeleteAsync(int playerId, int typeId)
        {
            var recipe = await FindAsync(playerId, typeId);
            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Row not found!");
            }
        }
    }

}
