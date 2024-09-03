using Backend.Data;
using Backend.Domain.Inputs.DTO;
using Backend.Domain.Modals.Spells;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class SpellStatisticRepository : ISpellStatisticRepository
    {
        private readonly GameDbContext _context;

        public SpellStatisticRepository(GameDbContext context)
        {
            _context = context;
        }

        public async Task<SpellStatistic?> FindAsync(int playerId, int typeId)
        {
            return await _context.SpellStatistics
                .FirstOrDefaultAsync(s => s.PlayerID == playerId && s.TypeID == typeId);
        }

        public async Task<IEnumerable<SpellStatistic>?> GetMultipleAsync(int playerId)
        {
            return await _context.SpellStatistics
                .Where(s => s.PlayerID == playerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<SpellStatistic>?> CreateAsync(int playerId, IEnumerable<int> typeIds)
        {
            var newSpellStatistics = typeIds.Select(typeId => new SpellStatistic
            {
                PlayerID = playerId,
                TypeID = typeId,
                Damage = 0,
                Kills = 0,
                Upgraded = 0,
                Cast = 0
            }).ToList();

            await _context.SpellStatistics.AddRangeAsync(newSpellStatistics);
            await _context.SaveChangesAsync();

            return newSpellStatistics;
        }

        public async Task<IEnumerable<SpellStatistic>?> UpdateAsync(int playerId, Dictionary<int, SpellStatisticUpdateInput> updatePairs)
        {
            var updatedSpellStatistics = new List<SpellStatistic>();
            foreach (var (typeId, updateData) in updatePairs)
            {
                var spellStatistic = await FindAsync(playerId, typeId);
                if (spellStatistic != null)
                {
                    spellStatistic.Damage += updateData.Damage;
                    spellStatistic.Kills += updateData.Kills;
                    spellStatistic.Upgraded += updateData.Upgraded;
                    spellStatistic.Cast += updateData.Cast;
                    updatedSpellStatistics.Add(spellStatistic);
                }
                else
                {
                    throw new Exception("Row not found!");
                }
            }

            await _context.SaveChangesAsync();
            return updatedSpellStatistics;
        }

        public async Task DeleteAsync(int playerId, int typeId)
        {
            var spellStatistic = await FindAsync(playerId, typeId);
            if (spellStatistic != null)
            {
                _context.SpellStatistics.Remove(spellStatistic);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Row not found!");
            }
        }
    }
}
