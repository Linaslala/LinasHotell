using LinasHotell.Models;
using LinasHotell.Services;
using System.Globalization;

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

            Console.WriteLine("=== REGISTRERA NYTT RUM ===\n");

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

                var normalized = pricePerNightInput.Trim().Replace('.', ',');

                if (decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.CurrentCulture, out pricePerNight)
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
                     && extraBedsAllowed >= 0 && extraBedsAllowed <= 2)
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
        public async Task UpdateRoomAsync()
        {
            Console.WriteLine("\nAnge rumsId på det rum du vill uppdatera:");

            if (!int.TryParse(Console.ReadLine(), out var roomId))
            {
                Console.WriteLine("Ogiltigt rumsId. Ange ett heltal.");
                return;
            }

            var room = await _roomService.GetRoomByIdAsync(roomId);

            if (room == null)
            {
                Console.WriteLine("Rummet du söker finns inte.");
                return;
            }

            Console.WriteLine($"\n=== Nuvarande rum ===\n{room}\n");

            while (true)
            {
                Console.Write($"Ange nytt rumsnummer (lämna blankt för att behålla nuvarande {room.RoomNumber}): ");
                var newRoomNumberInput = Console.ReadLine();


                if (string.IsNullOrWhiteSpace(newRoomNumberInput))
                    break;

                if (int.TryParse(newRoomNumberInput, out var newRoomNumber) &&
                                    newRoomNumber >= 1 &&
                                    newRoomNumber <= 999)
                {
                    room.RoomNumber = newRoomNumber;
                    break;
                }

                Console.WriteLine("Felaktigt rumsnummer. Ange endast siffror mellan 1 och 999.");
            }

            while (true)
            {
                Console.WriteLine($"Ange ny rumstyp (lämna blankt för att behålla nuvarande: {room.RoomType}):\n");
                Console.WriteLine("1 = Single");
                Console.WriteLine("2 = Double");
                Console.WriteLine("3 = Suite");

                var newRoomTypeInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(newRoomTypeInput))
                    break;

                if (int.TryParse(newRoomTypeInput, out var value) &&
                    Enum.IsDefined(typeof(RoomTypeEnums), value))
                {
                    room.RoomType = (RoomTypeEnums)value;
                    break;
                }

                Console.WriteLine("Ogiltig rumstyp. Ange 1, 2 eller 3.");
            }

            while (true)
            {
                Console.Write($"Ange nytt pris per natt (lämna blankt för att behålla nuvarande: {room.PricePerNight}kr/natt): ");
                var newPricePerNightInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(newPricePerNightInput))
                    break;

                var normalized = newPricePerNightInput.Trim().Replace('.', ',');

                if (decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.CurrentCulture, out var newPricePerNight)
                    && newPricePerNight >= 1)
                {
                    room.PricePerNight = newPricePerNight;
                    break;
                }

                Console.WriteLine("Felaktigt pris. Ange endast ett positivt belopp i siffror.");
            }

            while (true)
            {
                Console.Write($"Ange nytt antal extra sängar (0-2, lämna blankt för att behålla nuvarande: {room.ExtraBedsAllowed}): ");
                var newExtraBedsAllowedInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(newExtraBedsAllowedInput))
                    break;

                if (int.TryParse(newExtraBedsAllowedInput, out var newExtraBedsAllowed)
                   && newExtraBedsAllowed >= 0 && newExtraBedsAllowed <= 2)
                {

                    room.ExtraBedsAllowed = newExtraBedsAllowed;
                    break;

                }

                Console.WriteLine("Max två extrasängar är tillåtet. Ange 0, 1 eller 2.");
            }

            while (true)
            {
                Console.WriteLine($"Är rummet reod för bokningar? (J/N), Lämna blankt för att behålla nuvarande: {room.IsBookable}");
                var newIsBookableInput = (Console.ReadLine() ?? "").Trim().ToLower();

                if (string.IsNullOrWhiteSpace(newIsBookableInput))
                    break;

                if (newIsBookableInput == "j") { room.IsBookable = true; break; }
                if (newIsBookableInput == "n") { room.IsBookable = false; break; }

                Console.WriteLine("Ogiltigt svar. Ange J/j eller N/n.");
            }

            await _roomService.UpdateRoomAsync(room);

            Console.WriteLine("Rum uppdaterat!");

            //SKRIV UT SAMMANFATTNING AV UPPDATERINGEN
        }
        public async Task SetBookableRoomStatusAsync()
        {

        }
    }
}



