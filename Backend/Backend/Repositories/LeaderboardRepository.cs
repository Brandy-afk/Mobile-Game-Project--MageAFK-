using Backend.Data;
using Backend.Domain.Modals;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.Interfaces
{
    public class LeaderboardRepository : ILeaderboardRepository
    {
        private readonly GameDbContext _context;

        public LeaderboardRepository(GameDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Leaderboard>> GetLeaderboardByLocationAsync(int locationId)
        {
            return await _context.Leaderboard
                .Where(l => l.LocationID == locationId)
                .OrderByDescending(l => l.Wave)
                .Take(100)
                .ToListAsync();
        }

        public async Task<Leaderboard?> CreateLeaderboardEntryAsync(History historyEntry)
        {

            var entry = new Leaderboard()
            {
                LocationID = historyEntry.LocationID,
                PlayerID = historyEntry.PlayerID,
                Wave = historyEntry.Wave,
                Created = DateTime.UtcNow
            };

            _context.Leaderboard.Add(entry);
            await _context.SaveChangesAsync();
            await MaintainTop100EntriesAsync(entry.LocationID);
            return entry;
        }

       

        public async Task DeleteLeaderboardEntryAsync(int id)
        {
            var entry = await _context.Leaderboard.FindAsync(id);
            if (entry == null)
            {
                throw new Exception($"Leaderboard entry with ID {id} not found");
            }

            _context.Leaderboard.Remove(entry);
            await _context.SaveChangesAsync();
        }

        public async Task<int> MaintainTop100EntriesAsync(int locationId)
        {
            var entries = await _context.Leaderboard
                .Where(l => l.LocationID == locationId)
                .OrderByDescending(l => l.Wave)
                .ThenBy(l => l.Created)
                .Skip(100)
                .ToListAsync();

            _context.Leaderboard.RemoveRange(entries);
            return await _context.SaveChangesAsync();
        }
    }

}
