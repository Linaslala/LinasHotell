using LinasHotell.Models;
using LinasHotell.Services.ServiceInterfaces;
using LinasHotell.Utilities;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.IdentityModel.Tokens;
using Spectre.Console;

namespace LinasHotell.Controllers
{
    public class BookingController
    {
        private readonly IBookingService _bookingService;
        private readonly IGuestService _guestService;
        private readonly IRoomService _roomService;

        public BookingController(IBookingService bookingService, IGuestService guestService, IRoomService roomService)
        {
            _bookingService = bookingService;
            _guestService = guestService;
            _roomService = roomService;
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------
        public async Task ShowAllBookingsAsync()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();

            if (bookings.IsNullOrEmpty())
            {
                AnsiConsole.MarkupLine("[red]Det finns inga bokningar registrerade.[/]\n");

                AnsiConsole.MarkupLine("[grey]Tryck valfri tangent för att återgå till bokningsmenyn.[/]");
                AnsiConsole.Console.Input.ReadKey(false);
                AnsiConsole.Clear();
                return;
            }
            else
            {
                var table = new Table()
                    .Border(TableBorder.Rounded)
                    .ShowRowSeparators()
                    .AddColumn("Bokningsnummer", col => col.Centered())
                    .AddColumn("Gäst", col => col.NoWrap().RightAligned())
                    .AddColumn("Incheckningsdatum", col => col.Centered())
                    .AddColumn("Utcheckningsdatum", col => col.Centered())
                    .AddColumn("Rum", col => col.NoWrap().RightAligned())
                    .AddColumn("Extrasängar", col => col.Centered())
                    .AddColumn("Antal nätter", col => col.Centered())
                    .AddColumn("Pris kr", col => col.Centered());


                foreach (var booking in bookings.OrderBy(b => b.BookingId))
                {
                    table.AddRow(
                        booking.BookingId.ToString(),
                        booking.Guest?.ToString() ?? "",
                        booking.CheckInDate.ToString("yyyy-MM-dd"),
                        booking.CheckOutDate.ToString("yyyy-MM-dd"),
                        booking.Room?.ToString() ?? "",
                        booking.ExtraBeds.ToString(),
                        booking.Nights.ToString(),
                        booking.TotalPrice.ToString()
                    );
                }

                AnsiConsole.Write(table);
                Console.WriteLine();
            }

            AnsiConsole.MarkupLine("[grey]Tryck valfri tangent för att återgå till gästmenyn.[/]");
            AnsiConsole.Console.Input.ReadKey(false);
            AnsiConsole.Clear();
            return;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------
        public async Task AddBookingAsync()
        {
            var booking = new BookingModel();

            var addBookingTable = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("Gäst", col => col.Centered())
            .AddColumn("Incheckningsdatum", col => col.Centered())
            .AddColumn("Utcheckningsdatum", col => col.Centered())
            .AddColumn("Rum", col => col.Centered())
            .AddColumn("Extrasängar", col => col.Centered());

            AnsiConsole.Write(addBookingTable);

            while (true)
            {
                var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Skapa ny bokning")
                    .AddChoices(
                        "Boka med befintlig gäst",
                        "Avbryt"));

                switch (choice)
                {
                    case "Boka med befintlig gäst":

                        AnsiConsole.Clear();

                        var existingGuests = await _guestService.GetAllGuestsAsync();

                        if (existingGuests.Count == 0)
                        {
                            AnsiConsole.MarkupLine("[red]Det finns inga gäster registrerade.[/]\n");

                            AnsiConsole.MarkupLine("\n[grey]Tryck valfri tangent för att återgå till bokningsmenyn.[/]");
                            AnsiConsole.Console.Input.ReadKey(false);
                            AnsiConsole.Clear();
                            return;
                        }

                        var sortedGuests = existingGuests
                                .OrderBy(g => g.GuestId)
                                .ToList();

                        var table = new Table()
                                .Border(TableBorder.Rounded)
                                .AddColumn("GästId", col => col.Centered())
                                .AddColumn("Email", col => col.Centered())
                                .AddColumn("Förnamn", col => col.Centered())
                                .AddColumn("Efternamn", col => col.Centered())
                                .AddColumn("Telefon", col => col.Centered())
                                .AddColumn("Är incheckad", col => col.Centered());

                        foreach (var guest in sortedGuests.OrderBy(g => g.GuestId))
                        {
                            table.AddRow(guest.GuestId.ToString(),
                                guest.Email,
                                guest.FirstName,
                                guest.LastName,
                                guest.PhoneNumber.ToString(),
                                guest.IsCheckedIn ? "Ja" : "Nej");
                        }

                        AnsiConsole.Write(table);
                        Console.WriteLine();

                        var guestId = AnsiConsole.Prompt(
                            new TextPrompt<int>("Ange gästId:")
                                .ValidationErrorMessage("[red]Ogiltigt gästId.[/]")
                                .Validate(id => id > 0 && sortedGuests.Any(g => g.GuestId == id))
                        );

                        var selectedGuest = sortedGuests.First(g => g.GuestId == guestId);

                        booking.GuestId = selectedGuest.GuestId;
                        booking.Guest = selectedGuest;

                        AnsiConsole.Clear();

                        booking.CheckInDate = CalendarPicker.PickDate(DateTime.Today, "Välj incheckningsdatum");

                        booking.CheckOutDate = CalendarPicker.PickDate(booking.CheckInDate.AddDays(1), "Välj utcheckningsdatum");

                        AnsiConsole.Clear();

                        var availableRooms = await _roomService.GetAvailableRoomsAsync(
                             booking.CheckInDate,
                             booking.CheckOutDate);

                        if (availableRooms.Count == 0)
                        {
                            AnsiConsole.MarkupLine($"[red]Inga rum är tillgängliga för valda datum. {booking.CheckInDate:yyy-MM-dd} - {booking.CheckOutDate:yyy-MM-dd}[/] \n");

                            AnsiConsole.MarkupLine("[grey]Tryck valfri tangent för att välja ett nytt datum.[/]");
                            AnsiConsole.Console.Input.ReadKey(false);
                            AnsiConsole.Clear();
                            return;
                        }

                        AnsiConsole.Write(new Rule($"[yellow]Tillgängliga rum {booking.CheckOutDate:yyy-MM-dd} - {booking.CheckOutDate:yyy-MM-dd}[/]")
                        {
                            Justification = Justify.Left
                        });
                        AnsiConsole.WriteLine();

                        var availableRoomsTable = new Table()
                        .Border(TableBorder.Rounded)
                        .ShowRowSeparators()
                        .AddColumn("Rumsnummer", col => col.Centered())
                        .AddColumn("Rumstyp", col => col.Centered())
                        .AddColumn("Pris kr/natt", col => col.RightAligned())
                        .AddColumn("Max extrasängar", col => col.Centered());

                        foreach (var roomsAvailable in availableRooms.OrderBy(r => r.RoomNumber))
                        {
                            availableRoomsTable.AddRow(
                                roomsAvailable.RoomNumber.ToString(),
                                roomsAvailable.RoomType.ToString(),
                                roomsAvailable.PricePerNight.ToString(),
                                roomsAvailable.ExtraBedsAllowed.ToString());
                        }

                        AnsiConsole.Write(availableRoomsTable);

                        var selectedRoomNumber = AnsiConsole.Prompt(
                            new TextPrompt<int>("Ange rumsnummer: ")
                                .ValidationErrorMessage("[red]Ogiltigt rumsnummer.[/]")
                                .Validate(roomNumber => roomNumber > 0 && availableRooms.Any(r => r.RoomNumber == roomNumber))
                        );

                        var selectedRoom = availableRooms.First(r => r.RoomNumber == selectedRoomNumber);

                        booking.RoomId = selectedRoom.RoomId;
                        booking.Room = selectedRoom;

                        AnsiConsole.Clear();

                        var room = await _roomService.GetByRoomNumberAsync(selectedRoomNumber);

                        int maxExtraBeds = room.ExtraBedsAllowed;

                        if (maxExtraBeds == 0)
                        {
                            booking.ExtraBeds = 0;
                        }
                        else
                        {
                            var extraBeds = AnsiConsole.Prompt(
                                new TextPrompt<int>("Ange önskat antal extra sängar: ")
                                    .ValidationErrorMessage("[red]Ogiltigt antal extra sängar.[/]")
                                    .Validate(beds =>
                                        beds >= 0 && beds <= maxExtraBeds
                                            ? ValidationResult.Success()
                                            : ValidationResult.Error($"För {room.RoomType} är max {maxExtraBeds} extrasäng(ar).")));

                            booking.ExtraBeds = extraBeds;
                        }

                        AnsiConsole.Clear();

                        await _bookingService.AddBookingAsync(booking);

                        AnsiConsole.MarkupLine("[green]En ny bokning har skapats![/]");

                        var newBookingTable = new Table()
                            .Border(TableBorder.Rounded)
                            .AddColumn("Gäst", col => col.RightAligned())
                            .AddColumn("Incheckningsdatum", col => col.Centered())
                            .AddColumn("Utcheckningsdatum", col => col.Centered())
                            .AddColumn("Rum", col => col.RightAligned())
                            .AddColumn("Extrasängar", col => col.Centered())
                            .AddColumn("Antal nätter", col => col.Centered())
                            .AddColumn("Pris kr", col => col.Centered());

                        newBookingTable.AddRow(
                            booking.Guest?.ToString() ?? "",
                            booking.CheckInDate.ToString("yyyy-MM-dd"),
                            booking.CheckOutDate.ToString("yyyy-MM-dd"),
                            booking.Room?.ToString() ?? "",
                            booking.ExtraBeds.ToString(),
                            booking.Nights.ToString(),
                            booking.TotalPrice.ToString()
                        );

                        AnsiConsole.Write(newBookingTable);

                        AnsiConsole.MarkupLine("\n[grey]Tryck valfri tangent för att återgå till bokningsmenyn.[/]");
                        AnsiConsole.Console.Input.ReadKey(false);
                        AnsiConsole.Clear();
                        return;

                    case "Avbryt":
                        AnsiConsole.Clear();
                        return;
                }

            }
        }
        // ---------------------------------------------------------------------------------------------------------------------------------------------------
        public async Task UpdateBookingAsync()
        {
            var existingBookings = await _bookingService.GetAllBookingsAsync();

            if (existingBookings.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]Det finns inga bokningar registrerade.[/]\n");

                AnsiConsole.MarkupLine("\n[grey]Tryck valfri tangent för att återgå till bokningsmenyn.[/]");
                AnsiConsole.Console.Input.ReadKey(false);
                AnsiConsole.Clear();
                return;
            }

            var sortedBookings = existingBookings
                    .OrderBy(b => b.BookingId)
                    .ToList();

            var updateBookingTable = new Table()
                     .Border(TableBorder.Rounded)
                     .ShowRowSeparators()
                     .AddColumn("Bokningsnummer", col => col.Centered())
                     .AddColumn("Gäst", col => col.NoWrap().RightAligned())
                     .AddColumn("Incheckningsdatum", col => col.Centered())
                     .AddColumn("Utcheckningsdatum", col => col.Centered())
                     .AddColumn("Rum", col => col.NoWrap().RightAligned())
                     .AddColumn("Extrasängar", col => col.Centered())
                     .AddColumn("Antal nätter", col => col.Centered())
                     .AddColumn("Pris kr", col => col.Centered());


            foreach (var booking in existingBookings)
            {
                updateBookingTable.AddRow(
                    booking.BookingId.ToString(),
                    booking.Guest?.ToString() ?? "",
                    booking.CheckInDate.ToString("yyyy-MM-dd"),
                    booking.CheckOutDate.ToString("yyyy-MM-dd"),
                    booking.Room?.ToString() ?? "",
                    booking.ExtraBeds.ToString(),
                    booking.Nights.ToString(),
                    booking.TotalPrice.ToString()
                );
            }

            AnsiConsole.Write(updateBookingTable);
            Console.WriteLine();

            while (true)
            {
                var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Uppdatera befintlig bokning")
                    .AddChoices(
                        "Välj bokning (bokningsnummer)",
                        "Avbryt"));

                switch (choice)
                {
                    case "Välj bokning (bokningsnummer)":
                        var selectedBooking = Console.ReadLine();

                        if (!int.TryParse(selectedBooking, out int bookingId))
                        {
                            AnsiConsole.MarkupLine("\n[red]Ogiltigt bokningsnummer.[/]\n");
                            break;
                        }

                        var booking = existingBookings.FirstOrDefault(b => b.BookingId == bookingId);

                        AnsiConsole.Clear();

                        var bookingToUpdateTable = new Table()
                               .Border(TableBorder.Rounded)
                                .AddColumn("Bokningsnummer", col => col.Centered())
                                .AddColumn("Gäst", col => col.RightAligned())
                                .AddColumn("Incheckningsdatum", col => col.Centered())
                                .AddColumn("Utcheckningsdatum", col => col.Centered())
                                .AddColumn("Rum", col => col.RightAligned())
                                .AddColumn("Antal nätter", col => col.Centered())
                                .AddColumn("Pris kr", col => col.Centered());

                        bookingToUpdateTable.AddRow(
                            booking.BookingId.ToString(),
                            booking.Guest?.ToString() ?? "",
                            booking.CheckInDate.ToString("yyyy-MM-dd"),
                            booking.CheckOutDate.ToString("yyyy-MM-dd"),
                            booking.Room?.ToString() ?? "",
                            booking.Nights.ToString(),
                            booking.TotalPrice.ToString());

                        AnsiConsole.Write(bookingToUpdateTable);

                        var existingBooking = await _bookingService.GetAllBookingsAsync();
                        
                        booking.CheckInDate = CalendarPicker.PickDate(DateTime.Today, "Välj nytt incheckningsdatum");
                        
                        booking.CheckOutDate = CalendarPicker.PickDate(booking.CheckInDate.AddDays(1), "Välj nytt utcheckningsdatum");

                        AnsiConsole.Clear();

                        var availableRooms = await _roomService.GetAvailableRoomsAsync(
                            booking.CheckInDate,
                            booking.CheckOutDate);

                        if (availableRooms.Count == 0)
                        {
                            AnsiConsole.MarkupLine("[red]Inga rum är tillgängliga för valda datum.[/]\n");

                            AnsiConsole.MarkupLine("[grey]Tryck valfri tangent för att välja ett nytt datum.[/]");
                            AnsiConsole.Console.Input.ReadKey(false);
                            return;
                        }

                        AnsiConsole.Write(new Rule($"[yellow]Tillgängliga rum {booking.CheckOutDate:yyy-MM-dd} - {booking.CheckOutDate:yyy-MM-dd}[/]")
                        {
                            Justification = Justify.Left
                        });
                        AnsiConsole.WriteLine();

                        var availableRoomsTable = new Table()
                        .Border(TableBorder.Rounded)
                        .ShowRowSeparators()
                        .AddColumn("Rumsnummer", col => col.Centered())
                        .AddColumn("Rumstyp", col => col.Centered())
                        .AddColumn("Pris kr/natt", col => col.RightAligned())
                        .AddColumn("Max extrasängar", col => col.Centered());

                        foreach (var roomsAvailable in availableRooms.OrderBy(r => r.RoomNumber))
                        {
                            availableRoomsTable.AddRow(
                                roomsAvailable.RoomNumber.ToString(),
                                roomsAvailable.RoomType.ToString(),
                                roomsAvailable.PricePerNight.ToString(),
                                roomsAvailable.ExtraBedsAllowed.ToString());
                        }

                        AnsiConsole.Write(availableRoomsTable);

                        var selectedNewRoomNumber = AnsiConsole.Prompt(
                            new TextPrompt<int>("Ange nytt rumsNummer: ")
                                .ValidationErrorMessage("[red]Ogiltigt rumsnummer.[/]")
                                .Validate(roomNumber => roomNumber > 0 && availableRooms.Any(r => r.RoomNumber == roomNumber))
                        );

                        var selectedNewRoom = availableRooms.First(r => r.RoomNumber == selectedNewRoomNumber);

                        booking.RoomId = selectedNewRoom.RoomId;
                        booking.Room = selectedNewRoom;

                        AnsiConsole.Clear();

                        var room = await _roomService.GetByRoomNumberAsync(selectedNewRoomNumber);

                        int maxExtraBeds = room.ExtraBedsAllowed;

                        if (maxExtraBeds == 0)
                        {
                            booking.ExtraBeds = 0;
                        }
                        else
                        {
                            var newExtraBeds = AnsiConsole.Prompt(
                                new TextPrompt<int>("Ange nytt antal extra sängar: ")
                                    .ValidationErrorMessage("[red]Ogiltigt antal extra sängar.[/]")
                                    .Validate(beds =>
                                        beds >= 0 && beds <= maxExtraBeds
                                            ? ValidationResult.Success()
                                            : ValidationResult.Error($"För {room.RoomType} är max {maxExtraBeds} extrasäng(ar).")));

                            booking.ExtraBeds = newExtraBeds;
                        }

                        AnsiConsole.Clear();

                        await _bookingService.UpdateBookingAsync(booking);

                        AnsiConsole.MarkupLine("[green]Bokning har uppdaterats![/]");

                        var updatedBookingTable = new Table()
                            .Border(TableBorder.Rounded)
                            .AddColumn("Gäst", col => col.RightAligned())
                            .AddColumn("Incheckningsdatum", col => col.Centered())
                            .AddColumn("Utcheckningsdatum", col => col.Centered())
                            .AddColumn("Rum", col => col.RightAligned())
                            .AddColumn("Extrasängar", col => col.Centered())
                            .AddColumn("Antal nätter", col => col.Centered())
                            .AddColumn("Pris kr", col => col.Centered());

                        updatedBookingTable.AddRow(
                            booking.Guest?.ToString() ?? "",
                            booking.CheckInDate.ToString("yyyy-MM-dd"),
                            booking.CheckOutDate.ToString("yyyy-MM-dd"),
                            booking.Room?.ToString() ?? "",
                            booking.ExtraBeds.ToString(),
                            booking.Nights.ToString(),
                            booking.TotalPrice.ToString()
                        );

                        AnsiConsole.Write(updatedBookingTable);

                        AnsiConsole.MarkupLine("\n[grey]Tryck valfri tangent för att återgå till bokningsmenyn.[/]");
                        AnsiConsole.Console.Input.ReadKey(false);
                        AnsiConsole.Clear();
                        return;

                    case "Avbryt":
                        AnsiConsole.Clear();
                        return;
                }
            }
        }
        public async Task DeleteBookingAsync()
        {
            var existingBookings = await _bookingService.GetAllBookingsAsync();

            if (existingBookings.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]Det finns inga bokningar registrerade.[/]\n");

                AnsiConsole.MarkupLine("\n[grey]Tryck valfri tangent för att återgå till bokningsmenyn.[/]");
                AnsiConsole.Console.Input.ReadKey(false);
                AnsiConsole.Clear();
                return;
            }

            var sortedBookings = existingBookings
                    .OrderBy(b => b.BookingId)
                    .ToList();

            var updateBookingTable = new Table()
                     .Border(TableBorder.Rounded)
                     .ShowRowSeparators()
                     .AddColumn("Bokningsnummer", col => col.Centered())
                     .AddColumn("Gäst", col => col.NoWrap().RightAligned())
                     .AddColumn("Incheckningsdatum", col => col.Centered())
                     .AddColumn("Utcheckningsdatum", col => col.Centered())
                     .AddColumn("Rum", col => col.NoWrap().RightAligned())
                     .AddColumn("Extrasängar", col => col.Centered())
                     .AddColumn("Antal nätter", col => col.Centered())
                     .AddColumn("Pris kr", col => col.Centered());


            foreach (var booking in existingBookings)
            {
                updateBookingTable.AddRow(
                    booking.BookingId.ToString(),
                    booking.Guest?.ToString() ?? "",
                    booking.CheckInDate.ToString("yyyy-MM-dd"),
                    booking.CheckOutDate.ToString("yyyy-MM-dd"),
                    booking.Room?.ToString() ?? "",
                    booking.ExtraBeds.ToString(),
                    booking.Nights.ToString(),
                    booking.TotalPrice.ToString()
                );
            }

            AnsiConsole.Write(updateBookingTable);
            Console.WriteLine();

            while (true)
            {
                var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Radera bokning")
                    .AddChoices(
                        "Välj bokning att radera (bokningsnummer)",
                        "Avbryt"));

                switch (choice)
                {
                    case "Välj bokning att radera (bokningsnummer)":
                        var selectedBooking = Console.ReadLine();

                        if (!int.TryParse(selectedBooking, out int bookingId))
                        {
                            AnsiConsole.MarkupLine("\n[red]Ogiltigt bokningsnummer.[/]\n");
                            break;
                        }

                        var bookingToDelete = existingBookings.FirstOrDefault(b => b.BookingId == bookingId);

                        AnsiConsole.Clear();

                        var bookingToDeleteTable = new Table()
                               .Border(TableBorder.Rounded)
                                .AddColumn("Bokningsnummer", col => col.Centered())
                                .AddColumn("Gäst", col => col.RightAligned())
                                .AddColumn("Incheckningsdatum", col => col.Centered())
                                .AddColumn("Utcheckningsdatum", col => col.Centered())
                                .AddColumn("Rum", col => col.RightAligned())
                                .AddColumn("Antal nätter", col => col.Centered())
                                .AddColumn("Pris kr", col => col.Centered());

                        bookingToDeleteTable.AddRow(
                            bookingToDelete.BookingId.ToString(),
                            bookingToDelete.Guest?.ToString() ?? "",
                            bookingToDelete.CheckInDate.ToString("yyyy-MM-dd"),
                            bookingToDelete.CheckOutDate.ToString("yyyy-MM-dd"),
                            bookingToDelete.Room?.ToString() ?? "",
                            bookingToDelete.Nights.ToString(),
                            bookingToDelete.TotalPrice.ToString());

                        AnsiConsole.Write(bookingToDeleteTable);

                        var existingBooking = await _bookingService.GetAllBookingsAsync();

                        var sureToDeletePrompt = AnsiConsole.Prompt(
                            new SelectionPrompt<bool>()
                                .Title("\nÄr du säker på att du vill radera bokningen? (Hard delete)\n")
                                .AddChoices(true, false)
                                .UseConverter(value => value ? "Ja" : "Nej")
                        );

                        if (!sureToDeletePrompt)
                        {
                            AnsiConsole.Clear();
                            return;
                        }

                        await _bookingService.DeleteBookingAsync(bookingToDelete);

                        AnsiConsole.Clear();

                        AnsiConsole.MarkupLine("\n[green]Bokningen har raderats![/]\n");

                        var bookings = await _bookingService.GetAllBookingsAsync();

                        if (bookings.IsNullOrEmpty())
                        {
                            AnsiConsole.MarkupLine("[red]Det finns inga bokningar registrerade.[/]\n");

                            AnsiConsole.MarkupLine("[grey]Tryck valfri tangent för att återgå till bokningsmenyn.[/]");
                            AnsiConsole.Console.Input.ReadKey(false);
                            AnsiConsole.Clear();
                            return;
                        }
                        else
                        {
                            var table = new Table()
                            .Border(TableBorder.Rounded)
                            .ShowRowSeparators()
                            .AddColumn("Bokningsnummer", col => col.Centered())
                            .AddColumn("Gäst", col => col.NoWrap().RightAligned())
                            .AddColumn("Incheckningsdatum", col => col.Centered())
                            .AddColumn("Utcheckningsdatum", col => col.Centered())
                            .AddColumn("Rum", col => col.NoWrap().RightAligned())
                            .AddColumn("Extrasängar", col => col.Centered())
                            .AddColumn("Antal nätter", col => col.Centered())
                            .AddColumn("Pris kr", col => col.Centered());

                            foreach (var booking in bookings.OrderBy(b => b.BookingId))
                            {
                                table.AddRow(
                                    booking.BookingId.ToString(),
                                    booking.Guest?.ToString() ?? "",
                                    booking.CheckInDate.ToString("yyyy-MM-dd"),
                                    booking.CheckOutDate.ToString("yyyy-MM-dd"),
                                    booking.Room?.ToString() ?? "",
                                    booking.ExtraBeds.ToString(),
                                    booking.Nights.ToString(),
                                    booking.TotalPrice.ToString()
                                );
                            }

                            AnsiConsole.Write(table);
                            Console.WriteLine();
                        }
                        AnsiConsole.MarkupLine("\n[grey]Tryck valfri tangent för att återgå till gästmenyn.[/]");
                        AnsiConsole.Console.Input.ReadKey(false);
                        AnsiConsole.Clear();
                        return;

                    case "Avbryt":
                        AnsiConsole.Clear();
                        return;
                }
            }
        }
    }
}

