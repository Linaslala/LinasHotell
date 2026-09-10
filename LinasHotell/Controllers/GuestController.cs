using LinasHotell.Models;
using LinasHotell.Services.ServiceInterfaces;
using Microsoft.IdentityModel.Tokens;
using Spectre.Console;

namespace LinasHotell.Controllers
{
    public class GuestController
    {
        private readonly IGuestService _guestService;

        public GuestController(IGuestService guestService)
        {
            _guestService = guestService;
        }
        /// <summary>
        /// Displays a list of all registered guests asynchronously in a formatted table. If no guests are registered,
        /// shows an appropriate message to the user.
        /// </summary>
        /// <remarks>This method interacts with the console to present guest information and waits for
        /// user input before returning to the guest menu. The display includes guest details such as first name, last
        /// name, email, phone number, and check-in status.</remarks>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task ShowAllGuestsAsync()
        {
            var guests = await _guestService.GetAllGuestsAsync();

            if (guests.IsNullOrEmpty())
            {
                AnsiConsole.MarkupLine("[red]Det finns inga gäster registrerade.[/]\n");

                AnsiConsole.MarkupLine("[grey]Tryck valfri tangent för att återgå till gästmenyn.[/]");
                AnsiConsole.Console.Input.ReadKey(false);
                AnsiConsole.Clear();
                return;
            }
            else
            {
                var table = new Table()
                        .Border(TableBorder.Rounded)
                        .ShowRowSeparators()
                        .AddColumn("Förnamn", col => col.Centered())
                        .AddColumn("Efternamn", col => col.Centered())
                        .AddColumn("Email", col => col.Centered())
                        .AddColumn("Telefon", col => col.Centered())
                        .AddColumn("Är incheckad", col => col.Centered());

                foreach (var guest in guests.OrderBy(g => g.FirstName))
                {
                    table.AddRow(guest.FirstName,
                        guest.LastName,
                        guest.Email,
                        guest.PhoneNumber.ToString(),
                        guest.IsCheckedIn ? "Ja" : "Nej");
                }

                AnsiConsole.Write(table);
                Console.WriteLine();
            }

            AnsiConsole.MarkupLine("[grey]Tryck valfri tangent för att återgå till gästmenyn.[/]");
            AnsiConsole.Console.Input.ReadKey(false);
            AnsiConsole.Clear();
            return;
        }
        /// <summary>
        /// Prompts the user to enter guest information and adds a new guest to the system asynchronously.
        /// </summary>
        /// <remarks>This method interacts with the console to collect guest details, validates user
        /// input, and displays confirmation upon successful registration. The method prevents duplicate guests based on
        /// email address and enforces validation rules for each field.</remarks>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task AddGuestAsync()
        {
            var addGuestTable = new Table()
                        .Border(TableBorder.Rounded)
                        .ShowRowSeparators()
                        .AddColumn("Email", col => col.Centered())
                        .AddColumn("Förnamn", col => col.Centered())
                        .AddColumn("Efternamn", col => col.Centered())
                        .AddColumn("Telefon", col => col.Centered())
                        .AddColumn("Är incheckad", col => col.Centered());

            AnsiConsole.Write(addGuestTable);

            var guest = new GuestModel();

            var existingGuest = await _guestService.GetAllGuestsAsync();

            var guestEmail = AnsiConsole.Prompt(
                new TextPrompt<string>("Ange emailadress:")
                    .Validate(emailInput =>
                    {
                        var email = emailInput.Trim();

                        if (!IsValidEmail(email))
                            return ValidationResult.Error("Ogiltig e-postadress.");

                        if (existingGuest.Any(g =>
                                string.Equals(g.Email?.Trim(), email, StringComparison.OrdinalIgnoreCase)))
                        {
                            return ValidationResult.Error("Gästen finns redan i systemet.");
                        }

                        return ValidationResult.Success();
                    })
            );

            guest.Email = guestEmail;

            var firstName = AnsiConsole.Prompt(
                new TextPrompt<string>("Ange förnamn:")
                    .Validate(name =>
                    {
                        name = name?.Trim() ?? "";

                        if (name.Length > 120)
                            return ValidationResult.Error("Förnamn får inte vara längre än 120 tecken.");

                        if (name.Any(char.IsDigit))
                            return ValidationResult.Error("Förnamn får inte innehålla siffror.");

                        return ValidationResult.Success();
                    })
            );

            guest.FirstName = firstName;

            var lastName = AnsiConsole.Prompt(
                new TextPrompt<string>("Ange efternamn:")
                    .Validate(name =>
                    {
                        name = name?.Trim() ?? "";

                        if (name.Length > 120)
                            return ValidationResult.Error("Efternamn får inte vara längre än 120 tecken.");

                        if (name.Any(char.IsDigit))
                            return ValidationResult.Error("Efternamn får inte innehålla siffror.");

                        return ValidationResult.Success();
                    })
            );

            guest.LastName = lastName;

            var phoneNumber = AnsiConsole.Prompt(
                new TextPrompt<string>("Ange telefonnummer:")
                    .Validate(number =>
                    {
                        if (!number.All(char.IsDigit))
                            return ValidationResult.Error("Telefonnummer får bara innehålla siffror.");

                        if (number.Length > 30)
                            return ValidationResult.Error("Telefonnummer får inte vara längre än 30 siffror.");

                        return ValidationResult.Success();
                    })
            );

            guest.PhoneNumber = phoneNumber;

            var isCheckedIn = AnsiConsole.Prompt(
                            new SelectionPrompt<bool>()
                                .Title("Är gästen incheckad?\n")
                                .AddChoices(true, false)
                                .UseConverter(value => value ? "Ja" : "Nej")
                        );

            guest.IsCheckedIn = isCheckedIn;

            await _guestService.AddGuestAsync(guest);

            AnsiConsole.Clear();

            AnsiConsole.MarkupLine("[green]Gästen har registrerats![/]\n");

            var table = new Table()
                    .Border(TableBorder.Rounded)
                    .ShowRowSeparators()
                    .AddColumn("Email", col => col.Centered())
                    .AddColumn("Förnamn", col => col.Centered())
                    .AddColumn("Efternamn", col => col.Centered())
                    .AddColumn("Telefon", col => col.Centered())
                    .AddColumn("Är incheckad", col => col.Centered());

            table.AddRow(guest.Email,
                guest.FirstName,
                guest.LastName,
                guest.PhoneNumber.ToString(),
                guest.IsCheckedIn ? "Ja" : "Nej");

            AnsiConsole.Write(table);

            AnsiConsole.MarkupLine("\n[grey]Tryck valfri tangent för att återgå till gästmenyn.[/]");
            AnsiConsole.Console.Input.ReadKey(false);
            AnsiConsole.Clear();
            return;
        }
        /// <summary>
        /// Guides the user through updating the information of an existing guest by displaying a list of registered
        /// guests and prompting for new details.
        /// </summary>
        /// <remarks>If no guests are registered, the method notifies the user and returns without making
        /// changes. The method interacts with the console to display guest information and collect updated values.
        /// Input validation is performed for each field to ensure data integrity.</remarks>
        /// <returns>A task that represents the asynchronous update operation.</returns>
        public async Task UpdateGuestAsync()
        {
            var existingGuests = await _guestService.GetAllGuestsAsync();

            if (existingGuests.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]Det finns inga gäster registrerade.[/]\n");

                AnsiConsole.MarkupLine("\n[grey]Tryck valfri tangent för att återgå till gästmenyn.[/]");
                AnsiConsole.Console.Input.ReadKey(false);
                AnsiConsole.Clear();
                return;
            }

            var sortedGuests = existingGuests
                    .OrderBy(g => g.GuestId)
                    .ToList();

            var table = new Table()
                    .Border(TableBorder.Rounded)
                    .ShowRowSeparators()
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

            while (true)
            {
                var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Uppdatera gästinformation")
                    .AddChoices(
                        "Välj gäst (Id)",
                        "Avbryt"));

                switch (choice)
                {
                    case "Välj gäst (Id)":
                        var selectedGuest = Console.ReadLine();

                        if (!int.TryParse(selectedGuest, out int guestId))
                        {
                            AnsiConsole.MarkupLine("\n[red]Ogiltigt gästId.[/]\n");
                            break;
                        }

                        var guest = existingGuests.FirstOrDefault(g => g.GuestId == guestId);

                        AnsiConsole.Clear();

                        var guestToUpdateTable = new Table()
                            .Border(TableBorder.Rounded)
                            .ShowRowSeparators()
                            .AddColumn("Email", col => col.Centered())
                            .AddColumn("Förnamn", col => col.Centered())
                            .AddColumn("Efternamn", col => col.Centered())
                            .AddColumn("Telefon", col => col.Centered())
                            .AddColumn("Är incheckad", col => col.Centered());

                        guestToUpdateTable.AddRow(guest.Email,
                            guest.FirstName,
                            guest.LastName,
                            guest.PhoneNumber.ToString(),
                            guest.IsCheckedIn ? "Ja" : "Nej");

                        AnsiConsole.Write(guestToUpdateTable);

                        var existingGuest = await _guestService.GetAllGuestsAsync();

                        var newGuestEmail = AnsiConsole.Prompt(
                            new TextPrompt<string>("Ange emailadress:")
                                .Validate(emailInput =>
                                {
                                    var email = emailInput.Trim();

                                    if (!IsValidEmail(email))
                                        return ValidationResult.Error("Ogiltig e-postadress.");

                                    return ValidationResult.Success();
                                })
                        );

                        guest.Email = newGuestEmail;

                        var newFirstName = AnsiConsole.Prompt(
                            new TextPrompt<string>("Ange förnamn:")
                                .Validate(name =>
                                {
                                    name = name?.Trim() ?? "";

                                    if (name.Length > 120)
                                        return ValidationResult.Error("Förnamn får inte vara längre än 120 tecken.");

                                    if (name.Any(char.IsDigit))
                                        return ValidationResult.Error("Förnamn får inte innehålla siffror.");

                                    return ValidationResult.Success();
                                })
                            );

                        guest.FirstName = newFirstName;

                        var newLastName = AnsiConsole.Prompt(
                            new TextPrompt<string>("Ange efternamn:")
                                .Validate(name =>
                                {
                                    name = name?.Trim() ?? "";

                                    if (name.Length > 120)
                                        return ValidationResult.Error("Efternamn får inte vara längre än 120 tecken.");

                                    if (name.Any(char.IsDigit))
                                        return ValidationResult.Error("Efternamn får inte innehålla siffror.");

                                    return ValidationResult.Success();
                                })
                            );

                        guest.LastName = newLastName;

                        var newPhoneNumber = AnsiConsole.Prompt(
                            new TextPrompt<string>("Ange telefonnummer:")
                                .Validate(number =>
                                {
                                    if (!number.All(char.IsDigit))
                                        return ValidationResult.Error("Telefonnummer får bara innehålla siffror.");

                                    if (number.Length > 30)
                                        return ValidationResult.Error("Telefonnummer får inte vara längre än 30 siffror.");

                                    return ValidationResult.Success();
                                })
                            );

                        guest.PhoneNumber = newPhoneNumber;

                        var newIsCheckedIn = AnsiConsole.Prompt(
                                        new SelectionPrompt<bool>()
                                            .Title("Är gästen incheckad?\n")
                                            .AddChoices(true, false)
                                            .UseConverter(value => value ? "Ja" : "Nej")
                                    );

                        guest.IsCheckedIn = newIsCheckedIn;

                        await _guestService.UpdateGuestAsync(guest);

                        AnsiConsole.Clear();

                        AnsiConsole.MarkupLine("[green]Gästen har uppdaterats![/]\n");

                        var updatedGuestTable = new Table()
                            .Border(TableBorder.Rounded)
                            .ShowRowSeparators()
                            .AddColumn("Email", col => col.Centered())
                            .AddColumn("Förnamn", col => col.Centered())
                            .AddColumn("Efternamn", col => col.Centered())
                            .AddColumn("Telefon", col => col.Centered())
                            .AddColumn("Är incheckad", col => col.Centered());

                        updatedGuestTable.AddRow(guest.Email,
                            guest.FirstName,
                            guest.LastName,
                            guest.PhoneNumber.ToString(),
                            guest.IsCheckedIn ? "Ja" : "Nej");

                        AnsiConsole.Write(updatedGuestTable);

                        Console.WriteLine();

                        AnsiConsole.MarkupLine("[grey]Tryck valfri tangent för att återgå till gästmenyn.[/]");
                        AnsiConsole.Console.Input.ReadKey(false);
                        AnsiConsole.Clear();

                        return;

                    case "Avbryt":
                        AnsiConsole.Clear();
                        return;
                }

            }
        }
        /// <summary>
        /// Displays a list of registered guests and allows the user to update the check-in status of a selected guest
        /// asynchronously.
        /// </summary>
        /// <remarks>If no guests are registered, a message is displayed and the operation is canceled.
        /// The method presents an interactive prompt for selecting and updating a guest's check-in status. The console
        /// is cleared at various stages to enhance user experience.</remarks>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SetGuestStatusAsync()
        {
            var existingGuests = await _guestService.GetAllGuestsAsync();

            if (existingGuests.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]Det finns inga gäster registrerade.[/]\n");

                AnsiConsole.MarkupLine("[grey]Tryck valfri tangent för att återgå till gästmenyn.[/]");
                AnsiConsole.Console.Input.ReadKey(false);
                AnsiConsole.Clear();
                return;
            }

            var sortedGuests = existingGuests
                    .OrderBy(g => g.GuestId)
                    .ToList();

            var table = new Table()
                    .Border(TableBorder.Rounded)
                    .ShowRowSeparators()
                    .AddColumn("GästId", col => col.Centered())
                    .AddColumn("Email", col => col.Centered())
                    .AddColumn("Förnamn", col => col.Centered())
                    .AddColumn("Efternamn", col => col.Centered())
                    .AddColumn("Telefon", col => col.Centered())
                    .AddColumn("Är incheckad", col => col.Centered());

            foreach (var guest in sortedGuests.OrderBy(g => g.FirstName))
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

            while (true)
            {
                var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Välj gäst")
                    .AddChoices(
                        "Välj gäst (Id)",
                        "Avbryt"));

                switch (choice)
                {
                    case "Välj gäst (Id)":
                        var selectedGuest = Console.ReadLine();

                        if (!int.TryParse(selectedGuest, out int guestId))
                        {
                            AnsiConsole.MarkupLine("\n[red]Ogiltigt gästId.[/]\n");
                            break;
                        }

                        var guestStatus = existingGuests.FirstOrDefault(g => g.GuestId == guestId);

                        AnsiConsole.Clear();

                        var guestStatusToUpdateTable = new Table()
                            .Border(TableBorder.Rounded)
                            .ShowRowSeparators()
                            .AddColumn("Email", col => col.Centered())
                            .AddColumn("Förnamn", col => col.Centered())
                            .AddColumn("Efternamn", col => col.Centered())
                            .AddColumn("Telefon", col => col.Centered())
                            .AddColumn("Är incheckad", col => col.Centered());

                        guestStatusToUpdateTable.AddRow(guestStatus.Email,
                            guestStatus.FirstName,
                            guestStatus.LastName,
                            guestStatus.PhoneNumber.ToString(),
                            guestStatus.IsCheckedIn ? "Ja" : "Nej");

                        AnsiConsole.Write(guestStatusToUpdateTable);

                        var newGuestStatus = AnsiConsole.Prompt(
                            new SelectionPrompt<bool>()
                                .Title("Är gästen incheckad?\n")
                                .AddChoices(true, false)
                                .UseConverter(value => value ? "Ja" : "Nej")
                        );

                        guestStatus.IsCheckedIn = newGuestStatus;

                        await _guestService.UpdateGuestAsync(guestStatus);

                        AnsiConsole.Clear();

                        AnsiConsole.MarkupLine("[green]Gästens incheckningsstatus har uppdaterats![/]\n");

                        var guestUpdatedTable = new Table()
                        .Border(TableBorder.Rounded)
                        .ShowRowSeparators()
                        .AddColumn("Email", col => col.Centered())
                        .AddColumn("Förnamn", col => col.Centered())
                        .AddColumn("Efternamn", col => col.Centered())
                        .AddColumn("Telefon", col => col.Centered())
                        .AddColumn("Är incheckad", col => col.Centered());

                        guestUpdatedTable.AddRow(guestStatus.Email,
                            guestStatus.FirstName,
                            guestStatus.LastName,
                            guestStatus.PhoneNumber.ToString(),
                            guestStatus.IsCheckedIn ? "Ja" : "Nej");

                        AnsiConsole.Write(guestUpdatedTable);

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
        /// <summary>
        /// Deletes a guest from the system after user confirmation and validation checks.
        /// </summary>
        /// <remarks>This method displays a list of registered guests and prompts the user to select a
        /// guest to delete. A guest cannot be deleted if they are currently checked in or have active bookings. The
        /// method provides user feedback and confirmation prompts throughout the process. The guest list is refreshed
        /// after a successful deletion.</remarks>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        public async Task DeleteGuestAsync()
        {
            var existingGuests = await _guestService.GetAllGuestsAsync();

            if (existingGuests.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]Det finns inga gäster registrerade.[/]\n");

                AnsiConsole.MarkupLine("[grey]Tryck valfri tangent för att återgå till gästmenyn.[/]");
                AnsiConsole.Console.Input.ReadKey(false);
                AnsiConsole.Clear();
                return;
            }

            var sortedGuests = existingGuests
                    .OrderBy(g => g.GuestId)
                    .ToList();

            var table = new Table()
                    .Border(TableBorder.Rounded)
                    .ShowRowSeparators()
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

            while (true)
            {
                var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Radera gästinformation")
                    .AddChoices(
                        "Välj gäst (Id)",
                        "Avbryt"));

                switch (choice)
                {
                    case "Välj gäst (Id)":
                        var selectedGuest = Console.ReadLine();

                        if (!int.TryParse(selectedGuest, out int guestId))
                        {
                            AnsiConsole.MarkupLine("\n[red]Ogiltigt gästId.[/]\n");
                            break;
                        }

                        var guestToDelete = existingGuests.FirstOrDefault(g => g.GuestId == guestId);


                        if (guestToDelete.IsCheckedIn)
                        {
                            AnsiConsole.MarkupLine(
                                "\n[red]Gästen kan inte raderas eftersom den är incheckad.[/]\n"
                            );
                            AnsiConsole.MarkupLine(
                                "[grey]Checka ut gästen innan du försöker radera den.[/]"
                            );

                            AnsiConsole.Console.Input.ReadKey(false);
                            AnsiConsole.Clear();
                            return;
                        }


                        bool hasBookings = await _guestService.GuestHasBookingsAsync(guestToDelete.GuestId);

                        if (hasBookings)
                        {
                            AnsiConsole.MarkupLine(
                                "\n[red]Gästen kan inte raderas eftersom den har registrerade bokningar.[/]\n"
                            );
                            AnsiConsole.MarkupLine(
                                "[grey]Radera eller avsluta bokningarna först.[/]"
                            );

                            AnsiConsole.Console.Input.ReadKey(false);
                            AnsiConsole.Clear();
                            return;
                        }


                        AnsiConsole.Clear();

                        var guestToDeleteTable = new Table()
                            .Border(TableBorder.Rounded)
                            .ShowRowSeparators()
                            .AddColumn("Email", col => col.Centered())
                            .AddColumn("Förnamn", col => col.Centered())
                            .AddColumn("Efternamn", col => col.Centered())
                            .AddColumn("Telefon", col => col.Centered())
                            .AddColumn("Är incheckad", col => col.Centered());

                        guestToDeleteTable.AddRow(guestToDelete.Email,
                            guestToDelete.FirstName,
                            guestToDelete.LastName,
                            guestToDelete.PhoneNumber.ToString(),
                            guestToDelete.IsCheckedIn ? "Ja" : "Nej");

                        AnsiConsole.Write(guestToDeleteTable);

                        var sureToDeletePrompt = AnsiConsole.Prompt(
                            new SelectionPrompt<bool>()
                                .Title("\nÄr du säker på att du vill radera gästen? (Hard delete)\n")
                                .AddChoices(true, false)
                                .UseConverter(value => value ? "Ja" : "Nej")
                        );

                        if (!sureToDeletePrompt)
                        {
                            AnsiConsole.Clear();
                            return;
                        }

                        await _guestService.DeleteGuestAsync(guestToDelete);

                        AnsiConsole.Clear();

                        AnsiConsole.MarkupLine("\n[green]Gästens har raderats![/]\n");
                        var guests = await _guestService.GetAllGuestsAsync();

                        if (guests.IsNullOrEmpty())
                        {
                            AnsiConsole.MarkupLine("[red]Det finns inga gäster registrerade.[/]\n");

                            AnsiConsole.MarkupLine("[grey]Tryck valfri tangent för att återgå till gästmenyn.[/]");
                            AnsiConsole.Console.Input.ReadKey(false);
                            AnsiConsole.Clear();
                            return;
                        }
                        else
                        {
                            var registredGuestsTable = new Table()
                                    .Border(TableBorder.Rounded)
                                    .ShowRowSeparators()
                                    .AddColumn("Förnamn", col => col.Centered())
                                    .AddColumn("Efternamn", col => col.Centered())
                                    .AddColumn("Email", col => col.Centered())
                                    .AddColumn("Telefon", col => col.Centered())
                                    .AddColumn("Är incheckad", col => col.Centered());

                            foreach (var guest in guests.OrderBy(g => g.FirstName))
                            {
                                registredGuestsTable.AddRow(guest.FirstName,
                                    guest.LastName,
                                    guest.Email,
                                    guest.PhoneNumber.ToString(),
                                    guest.IsCheckedIn ? "Ja" : "Nej");
                            }

                            AnsiConsole.Write(registredGuestsTable);
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
        /// <summary>
        /// Determines whether the specified string is a valid email address format.
        /// </summary>
        /// <remarks>The validation checks for standard email address formatting and ignores leading or
        /// trailing whitespace. Email addresses ending with a period are considered invalid.</remarks>
        /// <param name="email">The email address to validate. Leading and trailing whitespace is ignored.</param>
        /// <returns>true if the email parameter is in a valid email address format; otherwise, false.</returns>
        public static bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith(".", StringComparison.Ordinal))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }
    }
}

