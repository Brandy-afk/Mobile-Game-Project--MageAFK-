using Backend.Domain.Modals;

namespace Backend.Repositories.Interfaces
{
    public interface IPlayerRepository
    {
        Task<Player?> GetPlayerByUsernameAsync(string username);
        Task<Player?> GetPlayerByIdAsync(int id);
        Task<Player> CreatePlayerAsync(string username);

        Task<Player?> DeletePlayerAsync(int playerId);
    }
}
