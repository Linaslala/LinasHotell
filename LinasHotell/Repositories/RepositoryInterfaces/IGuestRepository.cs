using LinasHotell.Models;

namespace LinasHotell.Repositories.RepositoryInterfaces
{
    public interface IGuestRepository
    {
        Task<GuestModel> AddAsync(GuestModel guest);
        Task DeleteAsync(GuestModel guestToDelete);
        Task<List<GuestModel>> GetAllAsync();
        Task<GuestModel?> GetByIdAsync(int guestId);
        Task<bool> HasBookingsAsync(int guestId);
        Task<GuestModel?> SetStatusAsync(int guestId, bool isCheckedIn);
        Task UpdateAsync(GuestModel guest);
    }
}