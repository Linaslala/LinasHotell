using LinasHotell.Models;

namespace LinasHotell.Repositories.Interfaces
{
    public interface IRoomRepository
    {
        RoomModel Add(RoomModel room);
        List<RoomModel> GetAll();
        RoomModel? GetById(int roomId);
        RoomModel? GetByRoomNumber(int roomNumber);
        void Delete(RoomModel rummet);
        void Update(RoomModel room);
    }
}