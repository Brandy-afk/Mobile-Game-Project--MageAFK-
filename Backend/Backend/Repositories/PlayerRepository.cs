using Backend.Data;
using Backend.Domain.Modals;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.Interfaces
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly GameDbContext _context;

        public PlayerRepository(GameDbContext context)
        {
            _context = context;
        }

        public async Task<Player?> GetPlayerByUsernameAsync(string username)
        {
            return await _context.Players
                .FirstOrDefaultAsync(p => p.Username == username);
        }

        public async Task<Player?> GetPlayerByIdAsync(int id) => await _context.Players.FindAsync(id);

        public async Task<Player> CreatePlayerAsync(string username)
        {
            var now = DateTime.UtcNow;
            var player = new Player
            {
                Username = username,
                CreatedAt = now,
                UpdatedAt = now
            };

            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            return player;
        }

        public async Task<Player?> DeletePlayerAsync(int playerId)
        {
            var player = await GetPlayerByIdAsync(playerId);
            if (player != null)
            {
                _context.Players.Remove(player);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Row not found!");
            }

            return player;
        }

 
    }
}
