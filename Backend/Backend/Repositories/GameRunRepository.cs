using Backend.Data;
using Backend.Domain.Modals;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.Interfaces
{
    public class GameRunRepository : IGameRunRepository
    {
        private readonly GameDbContext _context;

        public GameRunRepository(GameDbContext context)
        {
            _context = context;
        }

        public async Task<GameRun?> GetActiveGameRunAsync(int playerId)
        {
            return await _context.GameRuns
                .Where(gr => gr.PlayerID == playerId && gr.CompletedAt == null)
                .OrderByDescending(gr => gr.CreatedAt)
                .FirstOrDefaultAsync();
        }



        public async Task<GameRun> CreateGameRunAsync(int playerId, string gameState)
        {
            var gameRun = new GameRun
            {
                PlayerID = playerId,
                GameState = gameState,
                CreatedAt = DateTime.UtcNow
            };

            _context.GameRuns.Add(gameRun);
            await _context.SaveChangesAsync();

            return gameRun;
        }

        public async Task<GameRun?> CompleteGameRunAsync(int playerId)
        {
            var activeGameRun = await GetActiveGameRunAsync(playerId);
            if (activeGameRun == null)
            {
                throw new Exception($"No active game run found for player {playerId}");
            }

            activeGameRun.CompletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return activeGameRun;
        }

        public async Task<int> DeleteOldCompletedGameRunsAsync()
        {
            var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);
            var oldGameRuns = await _context.GameRuns
                .Where(gr => gr.CompletedAt != null && gr.CompletedAt < sevenDaysAgo)
                .ToListAsync();

            _context.GameRuns.RemoveRange(oldGameRuns);
            var deletedCount = await _context.SaveChangesAsync();

            return deletedCount;
        }
    }

}
