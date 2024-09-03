using Backend.Data;
using Backend.Domain.Modals.Milestones;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class MilestoneRepository : IMilestoneRepository
    {
        private readonly GameDbContext _context;

        public MilestoneRepository(GameDbContext context)
        {
            _context = context;
        }

        public async Task<Milestone?> FindAsync(int playerId, int typeId)
        {
            return await _context.Milestones
                .FirstOrDefaultAsync(m => m.PlayerID == playerId && m.TypeID == typeId);
        }

        public async Task<IEnumerable<Milestone>?> GetMultipleAsync(int playerId)
        {
            return await _context.Milestones
                .Where(m => m.PlayerID == playerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Milestone>?> CreateAsync(int playerId, IEnumerable<int> typeIds)
        {
            var newMilestones = typeIds.Select(typeId => new Milestone
            {
                PlayerID = playerId,
                TypeID = typeId,
                Value = 0,
                Rank = 0,
                RewardPoolSize = 0
            }).ToList();

            await _context.Milestones.AddRangeAsync(newMilestones);
            await _context.SaveChangesAsync();

            return newMilestones;
        }

        public async Task<IEnumerable<Milestone>?> UpdateAsync(int playerId, Dictionary<int, (float Value, int Rank, int RewardPoolSize)> updatePairs)
        {
            var updatedMilestones = new List<Milestone>();
            foreach (var (typeId, (value, rank, rewardPoolSize)) in updatePairs)
            {
                var milestone = await FindAsync(playerId, typeId);
                if (milestone != null)
                {
                    milestone.Value = value;
                    milestone.Rank = rank;
                    milestone.RewardPoolSize = rewardPoolSize;
                    updatedMilestones.Add(milestone);
                }
                else
                {
                    throw new Exception("Row not found!");
                }
            }

            await _context.SaveChangesAsync();
            return updatedMilestones;
        }

        public async Task DeleteAsync(int playerId, int typeId)
        {
            var milestone = await FindAsync(playerId, typeId);
            if (milestone != null)
            {
                _context.Milestones.Remove(milestone);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Row not found!");
            }
        }
    }

}
