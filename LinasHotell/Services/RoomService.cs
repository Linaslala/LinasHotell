using LinasHotell.Models;
using LinasHotell.Repositories.Interfaces;

namespace LinasHotell.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;

        public RoomService(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task<List<RoomModel>> GetAllRoomsAsync()
        {
            return await _roomRepository.GetAllAsync();
        }

        public async Task<RoomModel?> GetRoomByIdAsync(int roomId)
        {
            return await _roomRepository.GetByIdAsync(roomId);
        }

        public async Task<RoomModel> AddRoomAsync(RoomModel room)
        {
            var rooms = await _roomRepository.GetAllAsync();
            var roomExists = rooms.Any(r => r.RoomNumber == room.RoomNumber);

            if (roomExists)
                throw new Exception("Ett rum med detta rumsnummer finns redan.");

            return await _roomRepository.AddAsync(room);
        }
        public async Task UpdateRoomAsync(RoomModel room)
        {
            var rooms = await _roomRepository.GetAllAsync();
            var roomExists = rooms.Any(r => r.RoomNumber == room.RoomNumber);

            if (roomExists)
                throw new InvalidOperationException("Ett rum med detta rumsnummer finns redan.");

            await _roomRepository.UpdateAsync(room);
        }

        public async Task<RoomModel?> SetBookableRoomStatusAsync(int roomId, bool isBookable)
        {
            return await _roomRepository.SetBookableStatusAsync(roomId, isBookable);
        }
    }
}

