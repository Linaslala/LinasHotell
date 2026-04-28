using LinasHotell.Models;
using LinasHotell.Repositories.RepositoryInterfaces;
using LinasHotell.Services.ServiceInterfaces;

namespace LinasHotell.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<List<BookingModel>> GetAllBookingsAsync()
        {
            return await _bookingRepository.GetAllAsync();
        }

        public async Task<BookingModel?> GetBookingByIdAsync(int bookingId)
        {
            return await _bookingRepository.GetByIdAsync(bookingId);
        }

        public async Task<BookingModel> AddBookingAsync(BookingModel booking)
        {
            return await _bookingRepository.AddAsync(booking);
        }
        public async Task UpdateBookingAsync(BookingModel booking)
        {
            await _bookingRepository.UpdateAsync(booking);
        }

        public async Task DeleteBookingAsync(BookingModel bookingToDelete)
        {
            await _bookingRepository.DeleteAsync(bookingToDelete);
        }
    }
}
