using Backend.Data;
using Backend.Domain.Modals;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.Interfaces
{
    public class ElixirRepository : IElixirRepository
    {
        private readonly GameDbContext _context;

        public ElixirRepository(GameDbContext context)
        {
            _context = context;
        }

        public async Task<Elixir?> FindAsync(int playerId, int elixirId)
        {
            return await _context.ElixirShops
                .FirstOrDefaultAsync(e => e.PlayerID == playerId && e.ElixirID == elixirId);
        }

        public async Task<IEnumerable<Elixir>?> GetPlayerElixirsAsync(int playerId)
        {
            return await _context.ElixirShops
                .Where(e => e.PlayerID == playerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Elixir>?> CreateAsync(int playerId, IEnumerable<Elixir> elixirs)
        {   
            await _context.ElixirShops.AddRangeAsync(elixirs);
            await _context.SaveChangesAsync();

            return elixirs;
        }

        public async Task<Elixir?> UpdateAsync(int playerId, int elixirId, bool purchased)
        {
            var elixir = await FindAsync(playerId, elixirId);
            if (elixir != null)
            {
                elixir.Purchased = purchased;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Row not found!");
            }

            return elixir;
        }

        public async Task DeleteAsync(int playerId, int elixirId)
        {
            var elixir = await FindAsync(playerId, elixirId);
            if (elixir != null)
            {
                _context.ElixirShops.Remove(elixir);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Row not found!");
            }
        }
    }
}
