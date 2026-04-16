using LinasHotell.Models;
using LinasHotell.Repositories.Interfaces;

namespace LinasHotell.Services
{
    public class RoomService
    {
        private readonly IRoomRepository _roomRepository;

        public RoomService(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task<List<RoomModel>>? GetAllRoomsAsync()
        {
            return await _roomRepository.GetAllAsync();
        }

        public async Task<RoomModel>? GetRoomByIdAsync(int roomId)
        {
            return await _roomRepository.GetByIdAsync(roomId);
        }

    }
}
