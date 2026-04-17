using LinasHotell.Models;
using LinasHotell.Services.ServiceInterfaces;
using LinasHotell.Repositories.RepositoryInterfaces;

namespace LinasHotell.Services
{
    public class GuestService : IGuestService
    {
        private readonly IGuestRepository _guestRepository;

        public GuestService(IGuestRepository guestRepository)
        {
            _guestRepository = guestRepository;
        }

        public async Task<List<GuestModel>> GetAllGuestsAsync()
        {
            return await _guestRepository.GetAllAsync();
        }

        public async Task<GuestModel?> GetGuestByIdAsync(int guestId)
        {
            return await _guestRepository.GetByIdAsync(guestId);
        }

        public async Task<GuestModel> AddGuestAsync(GuestModel guest)
        {
            return await _guestRepository.AddAsync(guest);
        }
        public async Task UpdateGuestAsync(GuestModel guest)
        {
            await _guestRepository.UpdateAsync(guest);
        }

        public async Task<GuestModel?> SetGuestStatusAsync(int guestId, bool isCheckedIn)
        {
            return await _guestRepository.SetStatusAsync(guestId, isCheckedIn);
        }
    }
}
