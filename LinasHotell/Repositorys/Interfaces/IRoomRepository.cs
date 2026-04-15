using LinasHotell.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinasHotell.Repositorys.Interfaces
{
    public interface IRoomRepository
    {
        RoomModel? GetById(int roomId);
        RoomModel? GetByRoomNumber(int roomNumber);
        List<RoomModel> GetAll();
        RoomModel Add(RoomModel room);

        void Update(RoomModel room);
        void Delete(int roomId);
    }
}
