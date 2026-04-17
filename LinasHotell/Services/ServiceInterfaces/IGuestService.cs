using LinasHotell.Models;

namespace LinasHotell.Services.ServiceInterfaces
{
    public interface IGuestService
    {
        Task<GuestModel> AddGuestAsync(GuestModel guest);
        Task<List<GuestModel>> GetAllGuestsAsync();
        Task<GuestModel?> GetGuestByIdAsync(int guestId);
        Task<GuestModel?> SetGuestStatusAsync(int guestId, bool isCheckedIn);
        Task UpdateGuestAsync(GuestModel guest);
    }
}