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

            if (!rooms.Any())
            {
                Console.WriteLine("Det finns inga rum.");
                return;
            }

            //SPECTRE TABLE
            Console.WriteLine("=== Rummen ===\n");

            foreach (var room in rooms)
            {
                Console.WriteLine(room);
            }
        }

        public async Task ShowRoomDetailsAsync()
        {
            //SPECTRE TABLE ÖVER ALLA RUM: STYR MED PILTANGENTER FÖR ATT VÄLJA RUMSDETALJER
            
            Console.WriteLine("Ange rumsId för att se rummet");

            if (int.TryParse(Console.ReadLine(), out var roomId))
            {
                var room = await _roomService.GetRoomByIdAsync(roomId);

                if (room != null)
                {
                    Console.WriteLine($"\nDetaljer:\n{room}");
                }
                else
                {
                    Console.WriteLine("Rummet du söker finns inte.");
                }
            }
            else
            {
                Console.WriteLine("Ogiltligt rumsId.");
            }
        }
    }
}
