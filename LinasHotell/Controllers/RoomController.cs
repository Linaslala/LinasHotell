using LinasHotell.Models;
using LinasHotell.Services.ServiceInterfaces;
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
/// <summary>
/// Displays a list of all rooms in a formatted table asynchronously.
/// </summary>
/// <remarks>If no rooms are available, a message is displayed to inform the user. The method waits for user input
/// before returning to the previous menu.</remarks>
/// <returns>A task that represents the asynchronous operation.</returns>
        public async Task ShowAllRoomsAsync()
        {
            var rooms = await _roomService.GetAllRoomsAsync();

            if (rooms.IsNullOrEmpty())
            {
                AnsiConsole.MarkupLine("[Red]Det finns inga rum[/]");
            }
            else
            {
                var table = new Table()
                        .Border(TableBorder.Rounded)
                        .AddColumn("Rumsnummer", col => col.Centered())
                        .AddColumn("Rumstyp", col => col.Centered())
                        .AddColumn("Pris/natt", col => col.RightAligned())
                        .AddColumn("Max extrasängar", col => col.Centered())
                        .AddColumn("Bokningsbar", col => col.Centered());

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

/// <summary>
/// Guides the user through an interactive process to create and add a new room, prompting for room details and
/// validating input as needed.
/// </summary>
/// <remarks>This method prompts the user for room information, including room number, type, price per night,
/// allowed extra beds, and bookable status. Input is validated to ensure correctness and uniqueness. After successful
/// creation, the new room is added to the system and a summary is displayed. The method is intended for use in a
/// console application with interactive user input.</remarks>
/// <returns>A task that represents the asynchronous operation.</returns>
        public async Task AddRoomAsync()
        {
            var addRoomTable = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("Rumsnummer", col => col.Centered())
            .AddColumn("Rumstyp", col => col.Centered())
            .AddColumn("Pris/natt", col => col.RightAligned())
            .AddColumn("Max extrasängar", col => col.Centered())
            .AddColumn("Bokningsbar", col => col.Centered());

           addRoomTable.AddRow(" ", " ", " ", " ", " ");

            AnsiConsole.Write(addRoomTable);

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
                    .Title("Är rummet redo för bokningar?\n")
                    .AddChoices(true, false)
                    .UseConverter(value => value ? "Ja" : "Nej")
            );

            room.IsBookable = isBookable;

            await _roomService.AddRoomAsync(room);

            AnsiConsole.MarkupLine("[green]Rummet har skapats![/]\n");

            var table = new Table()
                                .Border(TableBorder.Rounded)
                                .AddColumn("Rumsnummer", col => col.Centered())
                                .AddColumn("Rumstyp", col => col.Centered())
                                .AddColumn("Pris/natt", col => col.RightAligned())
                                .AddColumn("Max extrasängar", col => col.Centered())
                                .AddColumn("Bokningsbar", col => col.Centered());

            table.AddRow(roomNumber.ToString(), roomType.ToString(), pricePerNight.ToString("0.00"), room.ExtraBedsAllowed.ToString(), isBookable ? "Ja" : "Nej");

            AnsiConsole.Write(table);

            AnsiConsole.MarkupLine("\nTryck valfri tangent för att återgå till rumsmenyn.");
            AnsiConsole.Console.Input.ReadKey(false);
            AnsiConsole.Clear();
        }

        /// <summary>
        /// Displays a list of rooms and allows the user to update the details of a selected room
        /// asynchronously.
        /// </summary>
        /// <remarks>If no rooms exists, the method notifies the user and returns immediately. The
        /// method prompts the user to select a room and update its details, including room number, type, price per
        /// night, allowed extra beds, and bookable status. Input validation is performed for each field to ensure valid
        /// data is provided. The updated room information is saved using the room service.</remarks>
        /// <returns>A task that represents the asynchronous update operation.</returns>
        public async Task UpdateRoomAsync()
        {
            var existingRooms = await _roomService.GetAllRoomsAsync();

            if (existingRooms.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]Inga rum hittades.[/]\n");
                return;
            }

            var sortedRooms = existingRooms
                    .OrderBy(r => r.RoomNumber)
                    .ToList();

            var table = new Table()
                   .Border(TableBorder.Rounded)
                   .AddColumn("Rumsnummer", col => col.Centered())
                   .AddColumn("Rumstyp", col => col.Centered())
                   .AddColumn("Pris/natt", col => col.RightAligned())
                   .AddColumn("Max extrasängar", col => col.Centered())
                   .AddColumn("Bokningsbar", col => col.Centered());

            foreach (var r in sortedRooms)
            {
                table.AddRow(
                    r.RoomNumber.ToString(),
                    r.RoomType.ToString(),
                    r.PricePerNight.ToString("0.00"),
                    r.ExtraBedsAllowed.ToString(),
                    r.IsBookable ? "Ja" : "Nej"
                );
            }

            AnsiConsole.Write(table);

            while (true)
            {
                var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Rumsmeny")
                    .AddChoices(
                        "Välj rum",
                        "Avbryt"));

                switch (choice)
                {
                    case "Välj rum":
                        var selectedRoom = Console.ReadLine();

                        if (!int.TryParse(selectedRoom, out int roomNumber))
                        {
                            AnsiConsole.MarkupLine("\n[red]Ogiltigt rumsnummer.[/]\n");
                            break;
                        }

                        var room = existingRooms.FirstOrDefault(r => r.RoomNumber == roomNumber);

                        AnsiConsole.Clear();

                        var roomToUpdateTable = new Table()
                        .Border(TableBorder.Rounded)
                        .AddColumn("Rumsnummer", col => col.Centered())
                        .AddColumn("Rumstyp", col => col.Centered())
                        .AddColumn("Pris/natt", col => col.RightAligned())
                        .AddColumn("Max extrasängar", col => col.Centered())
                        .AddColumn("Bokningsbar", col => col.Centered());

                        roomToUpdateTable.AddRow(
                            room.RoomNumber.ToString(),
                            room.RoomType.ToString(),
                            room.PricePerNight.ToString("0.00"),
                            room.ExtraBedsAllowed.ToString(),
                            room.IsBookable ? "Ja" : "Nej");

                        AnsiConsole.Write(roomToUpdateTable);

                        var newRoomNumber = AnsiConsole.Prompt(
                new TextPrompt<int>("Ange nytt rumsnummer:")
                    .Validate(number =>
                    {
                        if (number < 1 || number > 999)
                            return ValidationResult.Error("Rumsnummer måste vara mellan 1 och 999.");

                        if (existingRooms.Any(r => r.RoomNumber == number))
                            return ValidationResult.Error("Rumsnumret finns redan.");

                        return ValidationResult.Success();
                    })
            );

                        room.RoomNumber = newRoomNumber;

                        var newRoomType = AnsiConsole.Prompt(
                            new SelectionPrompt<RoomTypeEnums>()
                                .Title("Ange [green]rumstyp[/]:")
                                .AddChoices(
                                    RoomTypeEnums.Single,
                                    RoomTypeEnums.Double,
                                    RoomTypeEnums.Suite)
                        );

                        room.RoomType = newRoomType;

                        var newPricePerNight = AnsiConsole.Prompt(
                            new TextPrompt<decimal>("Ange [green]pris per natt[/]:")
                                .Validate(price =>
                                    price >= 1
                                        ? ValidationResult.Success()
                                        : ValidationResult.Error("Priset måste vara minst 1 kr."))
                                .Culture(CultureInfo.CurrentCulture)
                        );

                        room.PricePerNight = newPricePerNight;

                        int maxExtraBeds = newRoomType switch
                        {
                            RoomTypeEnums.Double => 1,
                            RoomTypeEnums.Suite => 2,
                            _ => 0
                        };

                        if (newRoomType != RoomTypeEnums.Single)
                        {
                            var newExtraBedsAllowed = AnsiConsole.Prompt(
                            new TextPrompt<int>($"Ange [green]extra sängar[/] (0–{maxExtraBeds}):")
                                .Validate(beds =>
                                    beds >= 0 && beds <= maxExtraBeds
                                        ? ValidationResult.Success()
                                        : ValidationResult.Error($"För {newRoomType} är max {maxExtraBeds} extrasäng(ar).")));

                            room.ExtraBedsAllowed = newExtraBedsAllowed;
                        }

                        var newIsBookable = AnsiConsole.Prompt(
                            new SelectionPrompt<bool>()
                                .Title("Är rummet redo för bokningar?\n")
                                .AddChoices(true, false)
                                .UseConverter(value => value ? "Ja" : "Nej")
                        );

                        room.IsBookable = newIsBookable;

                        await _roomService.UpdateRoomAsync(room);

                        AnsiConsole.MarkupLine("\n[green]Rummet har uppdaterats![/]\n");

                        var updatedRoomTable = new Table()
                        .Border(TableBorder.Rounded)
                        .AddColumn("Rumsnummer", col => col.Centered())
                        .AddColumn("Rumstyp", col => col.Centered())
                        .AddColumn("Pris/natt", col => col.RightAligned())
                        .AddColumn("Max extrasängar", col => col.Centered())
                        .AddColumn("Bokningsbar", col => col.Centered());

                        updatedRoomTable.AddRow(newRoomNumber.ToString(), newRoomType.ToString(), newPricePerNight.ToString("0.00"), room.ExtraBedsAllowed.ToString(), newIsBookable ? "Ja" : "Nej");

                        AnsiConsole.Write(updatedRoomTable);

                        AnsiConsole.MarkupLine("\nTryck valfri tangent för att återgå till rumsmenyn.");
                        AnsiConsole.Console.Input.ReadKey(false);
                        AnsiConsole.Clear();

                        break;

                    case "Avbryt":
                        AnsiConsole.Clear();
                        return;
                }

            }

        }

        /// <summary>
        /// Displays a list of rooms and allows the user to update the bookable status of a selected room
        /// asynchronously.
        /// </summary>
        /// <remarks>If no rooms exists, the method displays a message and returns immediately. The
        /// method interacts with the user via the console to select and update a room's bookable status. Changes are
        /// persisted using the room service.</remarks>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SetBookableRoomStatusAsync()
        {
            var existingRooms = await _roomService.GetAllRoomsAsync();

            if (existingRooms.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]Inga rum hittades.[/]\n");
                return;
            }

            var sortedRooms = existingRooms
                    .OrderBy(r => r.RoomNumber)
                    .ToList();

            var table = new Table()
                            .Border(TableBorder.Rounded)
                            .AddColumn("Rumsnummer", col => col.Centered())
                            .AddColumn("Rumstyp", col => col.Centered())
                            .AddColumn("Pris/natt", col => col.RightAligned())
                            .AddColumn("Max extrasängar", col => col.Centered())
                            .AddColumn("Bokningsbar", col => col.Centered());

            foreach (var r in sortedRooms)
            {
                table.AddRow(
                    r.RoomNumber.ToString(),
                    r.RoomType.ToString(),
                    r.PricePerNight.ToString("0.00"),
                    r.ExtraBedsAllowed.ToString(),
                    r.IsBookable ? "Ja" : "Nej"
                );
            }

            AnsiConsole.Write(table);

            while (true)
            {
                var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Uppdatera tillgänglighetsstatus")
                    .AddChoices(
                        "Välj rum",
                        "Avbryt"));

                switch (choice)
                {
                    case "Välj rum":
                        var selectedRoom = Console.ReadLine();

                        if (!int.TryParse(selectedRoom, out int roomNumber))
                        {
                            AnsiConsole.MarkupLine("\n[red]Ogiltigt rumsnummer.[/]\n");
                            break;
                        }

                        var roomStatusUpdate = existingRooms.FirstOrDefault(r => r.RoomNumber == roomNumber);

                        AnsiConsole.Clear();

                        var roomStatusUpdateTable = new Table()
                        .Border(TableBorder.Rounded)
                        .AddColumn("Rumsnummer", col => col.Centered())
                        .AddColumn("Rumstyp", col => col.Centered())
                        .AddColumn("Pris/natt", col => col.RightAligned())
                        .AddColumn("Max extrasängar", col => col.Centered())
                        .AddColumn("Bokningsbar", col => col.Centered());

                        roomStatusUpdateTable.AddRow(
                            roomStatusUpdate.RoomNumber.ToString(),
                            roomStatusUpdate.RoomType.ToString(),
                            roomStatusUpdate.PricePerNight.ToString("0.00"),
                            roomStatusUpdate.ExtraBedsAllowed.ToString(),
                            roomStatusUpdate.IsBookable ? "Ja" : "Nej");

                        AnsiConsole.Write(roomStatusUpdateTable);

                        var newIsBookableStatus = AnsiConsole.Prompt(
                            new SelectionPrompt<bool>()
                                .Title("Är rummet redo för bokningar?\n")
                                .AddChoices(true, false)
                                .UseConverter(value => value ? "Ja" : "Nej")
                        );

                        roomStatusUpdate.IsBookable = newIsBookableStatus;

                        await _roomService.UpdateRoomAsync(roomStatusUpdate);

                        AnsiConsole.MarkupLine("\n[green]Rummets tillgänglighetsstatus har uppdaterats![/]\n");

                        var updatedRoomTable = new Table()
                            .Border(TableBorder.Rounded)
                            .AddColumn("Rumsnummer", col => col.Centered())
                            .AddColumn("Rumstyp", col => col.Centered())
                            .AddColumn("Pris/natt", col => col.RightAligned())
                            .AddColumn("Max extrasängar", col => col.Centered())
                            .AddColumn("Bokningsbar", col => col.Centered());

                        updatedRoomTable.AddRow(
                            roomStatusUpdate.RoomNumber.ToString(),
                            roomStatusUpdate.RoomType.ToString(),
                            roomStatusUpdate.PricePerNight.ToString("0.00"),
                            roomStatusUpdate.ExtraBedsAllowed.ToString(),
                            roomStatusUpdate.IsBookable ? "Ja" : "Nej"
                        );

                        AnsiConsole.Write(updatedRoomTable);

                        AnsiConsole.MarkupLine("\nTryck valfri tangent för att återgå till rumsmenyn.");
                        AnsiConsole.Console.Input.ReadKey(false);
                        AnsiConsole.Clear();

                        break;

                    case "Avbryt":
                        AnsiConsole.Clear();
                        return;
                }
            }
        }
    }
}


