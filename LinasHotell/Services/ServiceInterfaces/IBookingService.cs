using LinasHotell.Models;

namespace LinasHotell.Services.ServiceInterfaces
{
    public interface IBookingService
    {
        Task<BookingModel> AddBookingAsync(BookingModel booking);
        Task DeleteBookingAsync(BookingModel bookingToDelete);
        Task<List<BookingModel>> GetAllBookingsAsync();
        Task<BookingModel?> GetBookingByIdAsync(int bookingId);
        Task UpdateBookingAsync(BookingModel booking);
    }
}