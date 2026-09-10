using LinasHotell.Models;
using LinasHotell.Repositories.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace LinasHotell.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _db;

        public BookingRepository(ApplicationDbContext db) => _db = db;

        public async Task<BookingModel?> GetByIdAsync(int bookingId)
        {
            var booking = await _db.Bookings
                .Where(b => b.BookingId == bookingId)
                .FirstOrDefaultAsync();

            return booking;
        }

        public async Task<List<BookingModel>> GetAllAsync()
        {
            var bookings = await _db.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Room)
                .OrderBy(b => b.BookingId)
                .ToListAsync();

            return bookings;
        }

        public async Task<BookingModel> AddAsync(BookingModel booking)
        {
            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync();

            return booking;
        }

        public async Task UpdateAsync(BookingModel booking)
        {
            _db.Bookings.Update(booking);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(BookingModel bookingToDelete)
        {
            if (bookingToDelete is null)
                return;

            _db.Bookings.Remove(bookingToDelete);
            await _db.SaveChangesAsync();
        }
    }
}
