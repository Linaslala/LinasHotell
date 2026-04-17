using LinasHotell.Models;
using LinasHotell.Repositories.RepositoryInterfaces;

namespace LinasHotell.Repositories
{
    public class GuestRepository : IGuestRepository
    {
        private readonly ApplicationDbContext _db;

        public GuestRepository(ApplicationDbContext db) => _db = db;

        public async Task<GuestModel?> GetByIdAsync(int guestId)
        {
            var guest = await _db.Guests
                .Where(g => g.GuestId == guestId)
                .FirstOrDefaultAsync();

            return guest;
        }

        //BEHÖVS VÄL BARA OM DET SKA FINNAS EN SÖKFUNKTION PÅ RUMSNUMMER???

        //public async Task<RoomModel?> GetByRoomNumberAsync(int roomNumber)
        //{
        //    var room = await _db.Rooms
        //     .Where(r => r.RoomNumber == roomNumber)
        //     .FirstOrDefaultAsync();

        //    return room;
        //}

        public async Task<List<GuestModel>> GetAllAsync()
        {
            var rooms = await _db.Guests
                .OrderBy(g => g.GuestId)
                .ToListAsync();

            return guests;
        }

        public async Task<GuestModel> AddAsync(GuestModel guest)
        {
            _db.Guests.Add(guest);
            await _db.SaveChangesAsync();

            return guest;
        }

        public async Task UpdateAsync(GuestModel guest)
        {
            _db.Guests.Update(guest);
            await _db.SaveChangesAsync();
        }

        public async Task<GuestModel?> SetStatusAsync(int guestId, bool isCheckedIn)
        {
            var guest = await GetByIdAsync(guestId);

            if (guest is null) return null;

            if (guest.IsCheckedIn == isCheckedIn)
                return guest;

            guest.IsCheckedIn = isCheckedIn;
            await UpdateAsync(guest);

            return guest;
        }
    }
}
