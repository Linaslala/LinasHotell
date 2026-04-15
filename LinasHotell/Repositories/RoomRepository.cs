
using LinasHotell.Models;
using LinasHotell.Repositories.Interfaces;

namespace LinasHotell.Repositorys
{
    public class RoomRepository : IRoomRepository
    {
        private readonly ApplicationDbContext _db;

        public RoomRepository(ApplicationDbContext db) => _db = db;

        public RoomModel? GetById(int roomId) => _db.Rooms.Find(roomId);

        public RoomModel? GetByRoomNumber(int roomNumber) =>
            _db.Rooms.FirstOrDefault(r => r.RoomNumber == roomNumber);

        public List<RoomModel> GetAll() => _db.Rooms
            .OrderBy(r => r.RoomId)
            .ToList();

        public RoomModel Add(RoomModel room)
        {
            _db.Rooms.Add(room);
            _db.SaveChanges();

            return room;
        }

        public void Update(RoomModel room)
        {
            _db.Rooms.Update(room);
            _db.SaveChanges();
        }

        public void Delete(RoomModel rummet)
        {
            var room = _db.Rooms.Find(rummet.RoomId);

            if (room is null)
            {

            }

            room.Deleted = DateTime.Now;

            Update(room);
        }
    }
}
