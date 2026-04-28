using LinasHotell.Controllers;
using Spectre.Console;

namespace LinasHotell.UIMenus
{
    public class BookingMenu
    {
        private readonly BookingController _bookingController;

        public BookingMenu(BookingController bookingController)
        {
            _bookingController = bookingController;
        }


        public async Task ShowBookingMenuAsync()
        {
            while (true)
            {
                var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Bokningsmeny")
                    .AddChoices(
                        "Visa alla bokningar",
                        "Skapa ny bokning",
                        "Uppdatera bokningsinformation",
                        "Ta bort bokning",
                        "Tillbaka till lobbyn"));

                switch (choice)
                {
                    case "Visa alla bokningar":
                        await _bookingController.ShowAllBookingsAsync();
                        break;

                    case "Skapa ny bokning":
                        await _bookingController.AddBookingAsync();
                        break;

                    case "Uppdatera bokningsinformation":
                        await _bookingController.UpdateBookingAsync();
                        break;

                    case "Ta bort bokning":
                        await _bookingController.DeleteBookingAsync();
                        break;

                    case "Tillbaka till lobbyn":
                        return;
                }
            }

        }
    }
}
