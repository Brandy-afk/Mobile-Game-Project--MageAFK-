using Backend.Data;
using Backend.Domain.Modals.Currency;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly GameDbContext _context;
        private readonly Dictionary<int, int> _initialValueMap;

        public CurrencyRepository(GameDbContext context)
        {
            _context = context;
            _initialValueMap = new Dictionary<int, int>
            {
                { 1, 100 }, 
                { 2, 0 },  

            };
        }

        public async Task<Currency?> FindAsync(int playerId, int typeId)
        {
            return await _context.Currencies
                .FirstOrDefaultAsync(c => c.PlayerID == playerId && c.TypeID == typeId);
        }

        public async Task<IEnumerable<Currency>?> GetMultipleAsync(int playerId)
        {
            return await _context.Currencies
                .Where(c => c.PlayerID == playerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Currency>?> CreateAsync(int playerId, IEnumerable<int> typeIds)
        {
            var newCurrencies = new List<Currency>();

            foreach (var typeId in typeIds)
            {
                var initialValue = _initialValueMap.TryGetValue(typeId, out var value) ? value : 0;

                var currency = new Currency
                {
                    PlayerID = playerId,
                    TypeID = typeId,
                    Value = initialValue
                };

                newCurrencies.Add(currency);
            }

            await _context.Currencies.AddRangeAsync(newCurrencies);
            await _context.SaveChangesAsync();

            return newCurrencies;
        }

        public async Task<IEnumerable<Currency>?> UpdateAsync(int playerId, Dictionary<int, int> valuePairs)
        {
            var updatedCurrencies = new List<Currency>();
            foreach (var (typeId, value) in valuePairs)
            {
                var currency = await FindAsync(playerId, typeId);

                if (currency != null)
                {
                    currency.Value = value;
                    updatedCurrencies.Add(currency);
                }
                else
                {
                    throw new Exception("Row not found! Currency not found!");
                }
            }

            await _context.SaveChangesAsync();
            return updatedCurrencies;
        }

        public async Task DeleteAsync(int playerId, int typeId)
        {
            var currency = await FindAsync(playerId, typeId);

            if (currency != null)
            {
                _context.Currencies.Remove(currency);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Row not found! Currency not found!");
            }
        }
    }
}
