using LinasHotell.Repositories.Interfaces;
using LinasHotell.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinasHotell.Controllers
{
    public class RoomController
    {
        private readonly RoomService _roomService;

        public RoomController(RoomService roomService)
        {
            _roomService = roomService;
        }

        public async Task ShowAllRoomsAsync()
        {
            var rooms = await _roomService.GetAllRoomsAsync();

            //SPECTRE TABLE
            Console.WriteLine("=== Rummen ===\n");

            foreach (var room in rooms)
            {
                Console.WriteLine(room);
            }
        }



    }
}
