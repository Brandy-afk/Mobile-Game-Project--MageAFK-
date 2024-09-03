using Backend.Domain.Modals;

namespace Backend.Repositories.Interfaces
{
    public interface IElixirRepository
    {
        Task<Elixir?> FindAsync(int playerId, int elixirId);
        Task<IEnumerable<Elixir>?> GetPlayerElixirsAsync(int playerId);
        Task<IEnumerable<Elixir>?> CreateAsync(int playerId, IEnumerable<Elixir> elixirs);
        Task<Elixir?> UpdateAsync(int playerId, int elixirId, bool purchased);
        Task DeleteAsync(int playerId, int elixirId);
    }
}
