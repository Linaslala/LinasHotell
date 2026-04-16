using LinasHotell.Models;
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

            Console.WriteLine("Ange rumsId för att se rummet: ");

            if (int.TryParse(Console.ReadLine(), out var roomId))
            {
                var room = await _roomService.GetRoomByIdAsync(roomId);

                if (room != null)
                {
                    Console.WriteLine($"\n=== Rummet ===\n{room}");
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

        public async Task AddRoomAsync()
        {
            int roomNumber;
            RoomTypeEnums roomType;
            decimal pricePerNight;
            int extraBedsAllowed;
            bool isBookable;

            while (true)
            {
                Console.Write("Ange rumsnummer: ");
                var roomNumberInput = Console.ReadLine();

                if (int.TryParse(roomNumberInput, out roomNumber) &&
                                    roomNumber >= 1 &&
                                    roomNumber <= 999)
                {
                    break;
                }

                Console.WriteLine("Felaktigt rumsnummer. Ange endast siffror mellan 1 och 999.");
            }

            while (true)
            {
                Console.WriteLine("Ange rumstyp:\n");
                Console.WriteLine("1 = Single");
                Console.WriteLine("2 = Double");
                Console.WriteLine("3 = Suite");
                var roomTypeInput = Console.ReadLine();

                if (int.TryParse(roomTypeInput, out var value) &&
                                        Enum.IsDefined(typeof(RoomTypeEnums), value))
                {
                    roomType = (RoomTypeEnums)value;
                    break;
                }

                Console.WriteLine("Ogiltig rumstyp. Ange 1, 2 eller 3.");
            }

            while (true)
            {
                Console.Write("Ange pris per natt: ");
                var pricePerNightInput = Console.ReadLine();

                if (decimal.TryParse(pricePerNightInput, out pricePerNight) 
                    && pricePerNight >= 1)
                {
                    break;
                }

                Console.WriteLine("Felaktigt pris. Ange endast ett positivt belopp i siffror.");
            }

            while (true)
            {
                Console.Write("Ange extra sängar (0-2): ");
                var extraBedsAllowedInput = Console.ReadLine();

                if (int.TryParse(extraBedsAllowedInput, out extraBedsAllowed)
                    && extraBedsAllowed <= 2)
                {
                    break;
                }

                Console.WriteLine("Max två extrasängar är tillåtet. Ange en siffra.");
            }

            while (true)
            {
                Console.WriteLine("Är rummet reod för bokningar? (J/N)");
                var isBookableInput = (Console.ReadLine() ?? "").Trim().ToLower();


                if (isBookableInput == "j") { isBookable = true; break; }
                if (isBookableInput == "n") { isBookable = false; break; }

                Console.WriteLine("Ogiltigt svar. Ange J/j eller N/n.");
            }

            var room = new RoomModel
            {
                RoomNumber = roomNumber,
                RoomType = roomType,
                PricePerNight = pricePerNight,
                ExtraBedsAllowed = extraBedsAllowed,
                IsBookable = isBookable
            };

            await _roomService.AddRoomAsync(room);

            Console.WriteLine("Nytt rum tillagt!");

            //SKRIV UT SAMMANSTÄLLNING AV NYTT RUM
        }
    }
}



