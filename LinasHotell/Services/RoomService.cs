using LinasHotell.Models;
using LinasHotell.Repositories.RepositoryInterfaces;
using LinasHotell.Services.ServiceInterfaces;

namespace LinasHotell.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IBookingRepository _bookingRepository;

        public RoomService(IRoomRepository roomRepository, IBookingRepository bookingRepository)
        {
            _roomRepository = roomRepository;
            _bookingRepository = bookingRepository;
        }

        public async Task<List<RoomModel>> GetAllRoomsAsync()
        {
            return await _roomRepository.GetAllAsync();
        }

        public async Task<RoomModel?> GetRoomByIdAsync(int roomId)
        {
            return await _roomRepository.GetByIdAsync(roomId);
        }

        public async Task<RoomModel?> GetByRoomNumberAsync(int roomNumber)
        {
            return await _roomRepository.GetByNumberAsync(roomNumber);
        }

        public async Task<RoomModel> AddRoomAsync(RoomModel room)
        {
            return await _roomRepository.AddAsync(room);
        }
        public async Task UpdateRoomAsync(RoomModel room)
        {
            await _roomRepository.UpdateAsync(room);
        }

        public async Task<RoomModel?> SetBookableRoomStatusAsync(int roomId, bool isBookable)
        {
            return await _roomRepository.SetBookableStatusAsync(roomId, isBookable);
        }


        public async Task<List<RoomModel>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, int? excludeBookingId = null)
        {
            if (checkOut <= checkIn) return new List<RoomModel>();

            var rooms = await _roomRepository.GetAllAsync();
            var bookings = await _bookingRepository.GetAllAsync();

            var unavailableRoomIds = bookings
                .Where(b =>
                    (excludeBookingId == null || b.BookingId != excludeBookingId) &&
                    b.CheckInDate < checkOut &&
                    b.CheckOutDate > checkIn)
                .Select(b => b.RoomId)
                .Distinct()
                .ToHashSet();

            return rooms
                .Where(r => r.IsBookable && !unavailableRoomIds.Contains(r.RoomId))
                .ToList();
        }
    }
}

