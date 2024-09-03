using Backend.Data;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Location = Backend.Domain.Modals.Location.Location;

namespace Backend.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly GameDbContext _context;

        public LocationRepository(GameDbContext context)
        {
            _context = context;
        }

        public async Task<Location?> FindAsync(int playerId, int typeId)
        {
            return await _context.Locations
                .FirstOrDefaultAsync(l => l.PlayerID == playerId && l.TypeID == typeId);
        }

        public async Task<IEnumerable<Location>?> GetMultipleAsync(int playerId)
        {
            return await _context.Locations
                .Where(l => l.PlayerID == playerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Location>?> CreateAsync(int playerId, IEnumerable<int> typeIds)
        {
            var newLocations = typeIds.Select(typeId => new Location
            {
                PlayerID = playerId,
                TypeID = typeId,
                BestWave = 0
            }).ToList();

            await _context.Locations.AddRangeAsync(newLocations);
            await _context.SaveChangesAsync();

            return newLocations;
        }

        public async Task<IEnumerable<Location>?> UpdateAsync(int playerId, Dictionary<int, int> bestWavePairs)
        {
            var updatedLocations = new List<Location>();
            foreach (var (typeId, bestWave) in bestWavePairs)
            {
                var location = await FindAsync(playerId, typeId);
                if (location != null)
                {
                    location.BestWave = bestWave;
                    updatedLocations.Add(location);
                }
                else
                {
                    throw new Exception("Row not found!");
                }
            }

            await _context.SaveChangesAsync();
            return updatedLocations;
        }

        public async Task DeleteAsync(int playerId, int typeId)
        {
            var location = await FindAsync(playerId, typeId);
            if (location != null)
            {
                _context.Locations.Remove(location);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Row not found!");
            }
        }

        
    }
}
