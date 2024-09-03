using Backend.Domain.Modals;

namespace Backend.Repositories.Interfaces
{
    public interface ILeaderboardRepository
    {
        Task<IEnumerable<Leaderboard>> GetLeaderboardByLocationAsync(int locationId);
        Task<Leaderboard?> CreateLeaderboardEntryAsync(History entry);
        Task DeleteLeaderboardEntryAsync(int id);
        Task<int> MaintainTop100EntriesAsync(int locationId);
    }
}
