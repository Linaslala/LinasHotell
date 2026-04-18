using LinasHotell.Models;
using LinasHotell.Services.ServiceInterfaces;
using Microsoft.IdentityModel.Tokens;
using Spectre.Console;
using System.Globalization;

namespace LinasHotell.Controllers
{
    public class GuestController
    {
        private readonly IGuestService _guestService;

        public GuestController(IGuestService guestService)
        {
            _guestService = guestService;
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------
        public async Task ShowAllGuestsAsync()
        {
            var guests = await _guestService.GetAllGuestsAsync();

            if (guests.IsNullOrEmpty())
            {
                AnsiConsole.MarkupLine("[Red]Det finns inga gäster[/]");
            }
            else
            {
                var table = new Table()
                        .Border(TableBorder.Rounded)
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

            Console.WriteLine("Tryck valfri tangent för att återgå till gästmenyn:");
            Console.ReadKey();
            Console.Clear();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------
        public async Task AddGuestAsync()
        {
            var addGuestTable = new Table()
                        .Border(TableBorder.Rounded)
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
                    .Validate(email =>
                    {
                        if (existingGuest.Any(g => g.Email == email))
                            return ValidationResult.Error("Gästen finns redan i systemet.");

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

            if (int.TryParse(phoneNumber, out var parsedNumber))
            {
                guest.PhoneNumber = parsedNumber;
            }

            var isCheckedIn = AnsiConsole.Prompt(
                            new SelectionPrompt<bool>()
                                .Title("Är gästen incheckad?\n")
                                .AddChoices(true, false)
                                .UseConverter(value => value ? "Ja" : "Nej")
                        );

            guest.IsCheckedIn = isCheckedIn;

            await _guestService.AddGuestAsync(guest);

            Console.WriteLine();

            AnsiConsole.MarkupLine("[green]Gästen har registrerats![/]\n");

            var table = new Table()
                    .Border(TableBorder.Rounded)
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

            AnsiConsole.MarkupLine("\nTryck valfri tangent för att återgå till rumsmenyn.");
            AnsiConsole.Console.Input.ReadKey(false);
            AnsiConsole.Clear();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------
        public async Task UpdateGuestAsync()
        {
            var existingGuests = await _guestService.GetAllGuestsAsync();

            if (existingGuests.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]Inga gäster hittades.[/]\n");
                return;
            }

            var sortedGuests = existingGuests
                    .OrderBy(g => g.FirstName)
                    .ToList();

            var table = new Table()
                    .Border(TableBorder.Rounded)
                    .AddColumn("[GästId]", col => col.Centered())
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

            AnsiConsole.Write(table);

            while (true)
            {
                var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Gästmeny")
                    .AddChoices(
                        "Välj gäst",
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

                        var newGuestEmail = AnsiConsole.Prompt(
                        new TextPrompt<string>("Ange emailadress:")
                            .Validate(email =>
                            {
                                if (existingGuests.Any(g => g.Email == email))
                                    return ValidationResult.Error("Gästen finns redan i systemet.");

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

                        if (int.TryParse(newPhoneNumber, out var newParsedNumber))
                        {
                            guest.PhoneNumber = newParsedNumber;
                        }

                        var newIsCheckedIn = AnsiConsole.Prompt(
                                        new SelectionPrompt<bool>()
                                            .Title("Är gästen incheckad?\n")
                                            .AddChoices(true, false)
                                            .UseConverter(value => value ? "Ja" : "Nej")
                                    );

                        guest.IsCheckedIn = newIsCheckedIn;

                        await _guestService.AddGuestAsync(guest);

                        Console.WriteLine();

                        AnsiConsole.MarkupLine("[green]Gästen har uppdaterats![/]\n");

                        var updatedGuestTable = new Table()
                            .Border(TableBorder.Rounded)
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

                        AnsiConsole.MarkupLine("\nTryck valfri tangent för att återgå till gästmenyn.");
                        AnsiConsole.Console.Input.ReadKey(false);
                        AnsiConsole.Clear();

                        break;

                    case "Avbryt":
                        AnsiConsole.Clear();
                        return;
                }

            }

        }
        //----------------------------------------------------------------------------------------------------------------------------------------------------
        public async Task SetGuestStatusAsync()
        {
            var existingGuests = await _guestService.GetAllGuestsAsync();

            if (existingGuests.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]Inga gäster hittades.[/]\n");
                return;
            }

            var sortedGuests = existingGuests
                    .OrderBy(g => g.GuestId)
                    .ToList();

            var table = new Table()
                    .Border(TableBorder.Rounded)
                    .AddColumn("[GästId]", col => col.Centered())
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

            AnsiConsole.Write(table);

            while (true)
            {
                var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Gästmeny")
                    .AddChoices(
                        "Välj gäst",
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

                        AnsiConsole.MarkupLine("\n[green]Gästens status har uppdaterats![/]\n");

                        var guestUpdatedTable = new Table()
                        .Border(TableBorder.Rounded)
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
        //----------------------------------------------------------------------------------------------------------------------------------------------------------
        public async Task DeleteGuestAsync()
        {
            var existingGuests = await _guestService.GetAllGuestsAsync();

            if (existingGuests.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]Inga gäster hittades.[/]\n");
                return;
            }

            var sortedGuests = existingGuests
                    .OrderBy(g => g.GuestId)
                    .ToList();

            var table = new Table()
                    .Border(TableBorder.Rounded)
                    .AddColumn("[GästId]", col => col.Centered())
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

            AnsiConsole.Write(table);

            while (true)
            {
                var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Gästmeny")
                    .AddChoices(
                        "Välj gäst",
                        "Avbryt"));

                switch (choice)
                {
                    case "Välj gäst [Id]":
                        var selectedGuest = Console.ReadLine();

                        if (!int.TryParse(selectedGuest, out int guestId))
                        {
                            AnsiConsole.MarkupLine("\n[red]Ogiltigt gästId.[/]\n");
                            break;
                        }

                        var guestToDelete = existingGuests.FirstOrDefault(g => g.GuestId == guestId);

                        AnsiConsole.Clear();

                        var guestToDeleteTable = new Table()
                            .Border(TableBorder.Rounded)
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
                                .Title("Är du säker på att du vill radera gästen? (Hard delete)\n")
                                .AddChoices(true, false)
                                .UseConverter(value => value ? "Ja" : "Nej")
                        );

                        if (!sureToDeletePrompt)
                        {
                            return;
                        }

                        await _guestService.DeleteGuestAsync(guestToDelete);

                        AnsiConsole.MarkupLine("\n[green]Gästens har raderats![/]\n");
                        break;

                    case "Avbryt":
                        AnsiConsole.Clear();
                        return;
                }
            }
        }
    }
}

