using LinasHotell.Models;
using LinasHotell.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LinasHotell.Repositorys
{
    public class RoomRepository : IRoomRepository
    {
        private readonly ApplicationDbContext _db;

        public RoomRepository(ApplicationDbContext db) => _db = db;

        public async Task<RoomModel?> GetByIdAsync(int roomId)
        {
            var room = await _db.Rooms
                .Where(r => r.RoomId == roomId)
                .FirstOrDefaultAsync();

            return room;
        }

        //BEHÖVS VÄL BARA OM DET SKA FINNAS EN SÖKFUNKTION PÅ RUMSNUMMER???

        //public async Task<RoomModel?> GetByRoomNumberAsync(int roomNumber)
        //{
        //    var room = await _db.Rooms
        //     .Where(r => r.RoomNumber == roomNumber)
        //     .FirstOrDefaultAsync();

        //    return room;
        //}

        public async Task<List<RoomModel>> GetAllAsync()
        {
            var rooms = await _db.Rooms
                .OrderBy(r => r.RoomId)
                .ToListAsync();

            return rooms;
        }

        public async Task<RoomModel> AddAsync(RoomModel room)
        {
            _db.Rooms.Add(room);
            await _db.SaveChangesAsync();

            return room;
        }

        public async Task UpdateAsync(RoomModel room)
        {
            _db.Rooms.Update(room);
            await _db.SaveChangesAsync();
        }

        public async Task<RoomModel?> SetBookableStatusAsync(int roomId, bool isBookable)
        {
            var room = await GetByIdAsync(roomId);

            if (room is null) return null;

            if (room.IsBookable == isBookable)
                return room;

            room.IsBookable = isBookable;
            await UpdateAsync(room);

            return room;
        }
    }
}
