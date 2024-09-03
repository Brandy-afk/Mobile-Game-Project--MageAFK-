using Backend.Data;
using Backend.Domain.Modals;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.Interfaces
{
    public class HistoryRepository : IHistoryRepository
    {
        private readonly GameDbContext _context;

        public HistoryRepository(GameDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<History>> GetPlayerHistoryAsync(int playerId)
        {
            return await _context.Histories
                .Where(h => h.PlayerID == playerId)
                .OrderByDescending(h => h.Date)
                .Take(10)
                .ToListAsync();
        }

        public async Task<History?> CreateHistoryEntryAsync(History entry)
        {
            entry.Date = DateTime.UtcNow;
            _context.Histories.Add(entry);
            await _context.SaveChangesAsync();
            await MaintainLatest10EntriesAsync(entry.PlayerID);
            return entry;
        }

        public async Task<int> MaintainLatest10EntriesAsync(int playerId)
        {
            var entriesToRemove = await _context.Histories
                .Where(h => h.PlayerID == playerId)
                .OrderByDescending(h => h.Date)
                .Skip(10)
                .ToListAsync();

            _context.Histories.RemoveRange(entriesToRemove);
            return await _context.SaveChangesAsync();
        }
    }
}
