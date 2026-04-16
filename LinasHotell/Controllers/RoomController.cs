using LinasHotell.Models;
using LinasHotell.Services;
using Microsoft.IdentityModel.Tokens;
using Spectre.Console;
using System.Globalization;

namespace LinasHotell.Controllers
{
    public class RoomController
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        public async Task ShowAllRoomsAsync()
        {
            var rooms = await _roomService.GetAllRoomsAsync();

            if (rooms.IsNullOrEmpty())
            {
                AnsiConsole.MarkupLine("[Red]Det finns inga rum[/]");
            }
            else
            {
                var table = new Table();

                table.AddColumn("Rumsnummer");
                table.AddColumn("Rumstyp");
                table.AddColumn("Pris/natt");
                table.AddColumn("Max antal extrasängar");
                table.AddColumn("Bokingsbar");

                foreach (var room in rooms.OrderBy(r => r.RoomNumber))
                {
                    table.AddRow(room.RoomNumber.ToString(), room.RoomType.ToString(), room.PricePerNight.ToString(), room.ExtraBedsAllowed.ToString(), room.IsBookable ? "Ja" : "Nej");
                }

                AnsiConsole.Write(table);
            }

            Console.WriteLine("Tryck valfri tangent för att återgå till rumsmenyn:");
            Console.ReadKey();
            Console.Clear();
        }

        public async Task AddRoomAsync()
        {
            var room = new RoomModel();

            var existingRooms = await _roomService.GetAllRoomsAsync();

            var roomNumber = AnsiConsole.Prompt(
                new TextPrompt<int>("Ange [green]rumsnummer[/]:")
                    .Validate(number =>
                    {
                        if (number < 1 || number > 999)
                            return ValidationResult.Error("Rumsnummer måste vara mellan 1 och 999.");

                        if (existingRooms.Any(r => r.RoomNumber == number))
                            return ValidationResult.Error("Rumsnumret finns redan.");

                        return ValidationResult.Success();
                    })
            );

            room.RoomNumber = roomNumber;

            var roomType = AnsiConsole.Prompt(
                new SelectionPrompt<RoomTypeEnums>()
                    .Title("Ange [green]rumstyp[/]:")
                    .AddChoices(
                        RoomTypeEnums.Single,
                        RoomTypeEnums.Double,
                        RoomTypeEnums.Suite)
            );

            room.RoomType = roomType;

            var pricePerNight = AnsiConsole.Prompt(
                new TextPrompt<decimal>("Ange [green]pris per natt[/]:")
                    .Validate(price =>
                        price >= 1
                            ? ValidationResult.Success()
                            : ValidationResult.Error("Priset måste vara minst 1 kr."))
                    .Culture(CultureInfo.CurrentCulture)
            );

            room.PricePerNight = pricePerNight;

            int maxExtraBeds = roomType switch
            {
                RoomTypeEnums.Double => 1,
                RoomTypeEnums.Suite => 2,
                _ => 0
            };

            if (roomType != RoomTypeEnums.Single)
            {
                var extraBedsAllowed = AnsiConsole.Prompt(
                new TextPrompt<int>($"Ange [green]extra sängar[/] (0–{maxExtraBeds}):")
                    .Validate(beds =>
                        beds >= 0 && beds <= maxExtraBeds
                            ? ValidationResult.Success()
                            : ValidationResult.Error($"För {roomType} är max {maxExtraBeds} extrasäng(ar).")));

                room.ExtraBedsAllowed = extraBedsAllowed;
            }

            var isBookable = AnsiConsole.Prompt(
                new SelectionPrompt<bool>()
                    .Title("Är rummet redo för bokningar?")
                    .AddChoices(true, false)
                    .UseConverter(value => value ? "Ja" : "Nej")
            );

            room.IsBookable = isBookable;

            await _roomService.AddRoomAsync(room);

            AnsiConsole.MarkupLine("[green]Rummet har skapats![/]");

            AnsiConsole.MarkupLine($"Rumsnummer: {room.RoomNumber}, " +
                $"Typ: {room.RoomType}, Pris: {room.PricePerNight} kr/natt, " +
                $"Möjligt antal extrasängar: {room.ExtraBedsAllowed}, " +
                $"Bokningsbart: {room.IsBookable}");

            AnsiConsole.MarkupLine("\nTryck valfri tangent för att återgå till rumsmenyn.");
            AnsiConsole.Console.Input.ReadKey(false);
            AnsiConsole.Clear();
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

                if (!string.IsNullOrWhiteSpace(newRoomTypeInput))
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

            try
            {
                await _roomService.UpdateRoomAsync(room);
                Console.WriteLine("Rum uppdaterat!");

            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            //SKRIV UT SAMMANFATTNING AV UPPDATERINGEN
        }
        public async Task SetBookableRoomStatusAsync()
        {
            //SPECTRE TABLE FÖR ATT VÄLJA RUM

            Console.Write("Ange rumsId för att ändra status på tillgänglighet: ");

            if (!int.TryParse(Console.ReadLine(), out var roomId))
            {
                Console.WriteLine("Ogiltigt rumsId.");
                return;
            }

            var room = await _roomService.SetBookableRoomStatusAsync(roomId, false);

            if (room == null)
            {
                Console.WriteLine("Rummet hittades inte.");
                return;
            }

            Console.WriteLine($"Rum {room.RoomNumber} är nu EJ bokningsbart.");
        }
    }
}



