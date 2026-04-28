using LinasHotell.UIMenus;

public class MenuNavigator
{
    private readonly MainMenu _mainMenu;
    private readonly RoomMenu _roomMenu;
    private readonly GuestMenu _guestMenu;
    private readonly BookingMenu _bookingMenu;

    public MenuNavigator(MainMenu mainMenu, RoomMenu roomMenu, GuestMenu guestMenu, BookingMenu bookingMenu)
    {
        _mainMenu = mainMenu;
        _roomMenu = roomMenu;
        _guestMenu = guestMenu;
        _bookingMenu = bookingMenu;
    }

    public async Task RunAsync()
    {
        while (true)
        {
            var choice = await _mainMenu.ShowAsync();

            switch (choice)
            {
                case MainMenuChoice.Rum:
                    await _roomMenu.ShowRoomMenuAsync();
                    break;

                case MainMenuChoice.Gäst:
                    await _guestMenu.ShowGuestMenuAsync();
                    break;

                case MainMenuChoice.Bokning:
                    await _bookingMenu.ShowBookingMenuAsync();
                    break;

                case MainMenuChoice.Avsluta:
                    Environment.Exit(0);
                    break;
            }
        }
    }
}
