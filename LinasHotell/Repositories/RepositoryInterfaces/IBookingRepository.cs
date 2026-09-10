using LinasHotell.Models;

namespace LinasHotell.Repositories.RepositoryInterfaces
{
    public interface IBookingRepository
    {
        Task<BookingModel> AddAsync(BookingModel booking);
        Task DeleteAsync(BookingModel bookingToDelete);
        Task<List<BookingModel>> GetAllAsync();
        Task<BookingModel?> GetByIdAsync(int bookingId);
        Task UpdateAsync(BookingModel booking);
    }
}