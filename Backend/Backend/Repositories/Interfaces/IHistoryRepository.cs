using Backend.Domain.Modals;

namespace Backend.Repositories.Interfaces
{
    public interface IHistoryRepository
    {
        Task<IEnumerable<History>> GetPlayerHistoryAsync(int playerId);
        Task<History?> CreateHistoryEntryAsync(History entry);
        Task<int> MaintainLatest10EntriesAsync(int playerId);
    }
}
