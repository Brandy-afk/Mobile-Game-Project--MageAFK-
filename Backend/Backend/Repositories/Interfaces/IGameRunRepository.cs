using Backend.Domain.Modals;

namespace Backend.Repositories.Interfaces
{
    public interface IGameRunRepository
    {
        Task<int> DeleteOldCompletedGameRunsAsync();
        Task<GameRun?> GetActiveGameRunAsync(int playerId);
        Task<GameRun> CreateGameRunAsync(int playerId, string gameState);
        Task<GameRun?> CompleteGameRunAsync(int playerId);
    }
}
