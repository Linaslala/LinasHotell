using LinasHotell.Models;

namespace LinasHotell.Repositories.RepositoryInterfaces
{
    public interface IRoomRepository
    {
        Task<RoomModel> AddAsync(RoomModel room);
        Task<List<RoomModel>> GetAllAsync();
        Task<RoomModel?> GetByIdAsync(int roomId);
        Task<RoomModel?> GetByNumberAsync(int roomNumber);
        Task<RoomModel?> SetBookableStatusAsync(int roomId, bool isBookable);
        Task UpdateAsync(RoomModel room);
    }
}