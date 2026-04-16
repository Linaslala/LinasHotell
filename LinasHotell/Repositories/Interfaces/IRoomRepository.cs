using LinasHotell.Models;

namespace LinasHotell.Repositories.Interfaces
{
    public interface IRoomRepository
    {
        Task<RoomModel> AddAsync(RoomModel room);
        Task<List<RoomModel>> GetAllAsync();
        Task<RoomModel?> GetByIdAsync(int roomId);
        //Task<RoomModel?> GetByRoomNumberAsync(int roomNumber);
        Task<RoomModel?> SetBookableStatusAsync(int roomId, bool isBookable);
        Task UpdateAsync(RoomModel room);
    }
}