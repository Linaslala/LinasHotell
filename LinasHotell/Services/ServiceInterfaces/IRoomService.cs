using LinasHotell.Models;

namespace LinasHotell.Services.ServiceInterfaces
{
    public interface IRoomService
    {
        Task<RoomModel> AddRoomAsync(RoomModel room);
        Task<List<RoomModel>> GetAllRoomsAsync();
        Task<RoomModel?> GetRoomByIdAsync(int roomId);
        Task<RoomModel?> SetBookableRoomStatusAsync(int roomId, bool isBookable);
        Task UpdateRoomAsync(RoomModel room);
    }
}