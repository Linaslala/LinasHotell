using LinasHotell.Controllers;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinasHotell.UIMenus
{
    public class GuestMenu
    {
        private readonly GuestController _guestController;

        public GuestMenu(GuestController guestController)
        {
            _guestController = guestController;
        }


        public async Task ShowGuestMenuAsync()
        {
            while (true)
            {
                var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Gästemeny")
                    .AddChoices(
                        "Visa alla gäster",
                        "Registrera ny gäst",
                        "Uppdatera gästinformation",
                        "Checka in/ut gäst",
                        "Tillbaka till lobbyn"));

                switch (choice)
                {
                    case "Visa alla gäster":
                        await _guestController.ShowAllGuestsAsync();
                        break;

                    case "Registrera ny gäst":
                        await _guestController.AddGuestAsync();
                        break;

                    case "Uppdatera gästinformation":
                        await _guestController.UpdateGuestAsync();
                        break;

                    case "Checka in/ut gäst":
                        await _guestController.SetGuestStatusAsync();
                        break;

                    case "Tillbaka till lobbyn":
                        return;
                }
            }

        }
    }

    public enum GuestMenuResult
    {
        Stay,
        BackToLobby
    }
}

