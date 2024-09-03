using Backend.Data;
using Backend.Domain.Modals.Skills;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class SkillRepository : ISkillRepository
    {
        private readonly GameDbContext _context;

        public SkillRepository(GameDbContext context)
        {
            _context = context;
        }

        public async Task<Skill?> FindAsync(int playerId, int typeId)
        {
            return await _context.Skills
                .FirstOrDefaultAsync(s => s.PlayerID == playerId && s.TypeID == typeId);
        }

        public async Task<IEnumerable<Skill>?> GetMultipleAsync(int playerId)
        {
            return await _context.Skills
                .Where(s => s.PlayerID == playerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Skill>?> CreateAsync(int playerId, IEnumerable<int> typeIds)
        {
            var newSkills = typeIds.Select(typeId => new Skill
            {
                PlayerID = playerId,
                TypeID = typeId,
                Rank = 0
            }).ToList();

            await _context.Skills.AddRangeAsync(newSkills);
            await _context.SaveChangesAsync();

            return newSkills;
        }

        public async Task<IEnumerable<Skill>?> UpdateAsync(int playerId, Dictionary<int, int> updatePairs)
        {
            var updatedSkills = new List<Skill>();
            foreach (var (typeId, rank) in updatePairs)
            {
                var skill = await FindAsync(playerId, typeId);
                if (skill != null)
                {
                    skill.Rank = rank;
                    updatedSkills.Add(skill);
                }
                else
                {
                    throw new Exception("Row not found!");
                }
            }

            await _context.SaveChangesAsync();
            return updatedSkills;
        }

        public async Task DeleteAsync(int playerId, int typeId)
        {
            var skill = await FindAsync(playerId, typeId);
            if (skill != null)
            {
                _context.Skills.Remove(skill);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Row not found!");
            }
        }
    }
}
