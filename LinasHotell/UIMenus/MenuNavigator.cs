using LinasHotell.UIMenus;

public class MenuNavigator
{
    private readonly MainMenu _mainMenu;
    private readonly RoomMenu _roomMenu;

    public MenuNavigator(MainMenu mainMenu, RoomMenu roomMenu)
    {
        _mainMenu = mainMenu;
        _roomMenu = roomMenu;
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

                case MainMenuChoice.Avsluta:
                    Environment.Exit(0);
                    break;
            }
        }
    }
}
