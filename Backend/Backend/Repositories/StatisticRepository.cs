using Backend.Data;
using Backend.Domain.Modals.PlayerStatistics;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class StatisticRepository : IStatisticRepository
    {
        private readonly GameDbContext _context;

        public StatisticRepository(GameDbContext context)
        {
            _context = context;
        }

        public async Task<Statistic?> FindAsync(int playerId, int typeId)
        {
            return await _context.PlayerStatistics
                .FirstOrDefaultAsync(s => s.PlayerID == playerId && s.TypeID == typeId);
        }

        public async Task<IEnumerable<Statistic>?> GetMultipleAsync(int playerId)
        {
            return await _context.PlayerStatistics
                .Where(s => s.PlayerID == playerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Statistic>?> CreateAsync(int playerId, IEnumerable<int> typeIds)
        {
            var newStatistics = typeIds.Select(typeId => new Statistic
            {
                PlayerID = playerId,
                TypeID = typeId,
                Value = 0
            }).ToList();

            await _context.PlayerStatistics.AddRangeAsync(newStatistics);
            await _context.SaveChangesAsync();

            return newStatistics;
        }

        public async Task<IEnumerable<Statistic>?> UpdateAsync(int playerId, Dictionary<int, float> valuePairs)
        {
            var updatedStatistics = new List<Statistic>();
            foreach (var (typeId, value) in valuePairs)
            {
                var statistic = await FindAsync(playerId, typeId);
                if (statistic != null)
                {
                    statistic.Value = value;
                    updatedStatistics.Add(statistic);
                }
                else
                {
                    throw new Exception("Row not found!");
                }
            }

            await _context.SaveChangesAsync();
            return updatedStatistics;
        }

        public async Task DeleteAsync(int playerId, int typeId)
        {
            var statistic = await FindAsync(playerId, typeId);
            if (statistic != null)
            {
                _context.PlayerStatistics.Remove(statistic);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Row not found!");
            }
        }
    }
}
