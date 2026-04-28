using LinasHotell.Models;

namespace LinasHotell.Services.ServiceInterfaces
{
    public interface IGuestService
    {
        Task<GuestModel> AddGuestAsync(GuestModel guest);
        Task DeleteGuestAsync(GuestModel guestToDelete);
        Task<List<GuestModel>> GetAllGuestsAsync();
        Task<GuestModel?> GetGuestByIdAsync(int guestId);
        Task<bool> GuestHasBookingsAsync(int guestId);
        Task<GuestModel?> SetGuestStatusAsync(int guestId, bool isCheckedIn);
        Task UpdateGuestAsync(GuestModel guest);
    }
}