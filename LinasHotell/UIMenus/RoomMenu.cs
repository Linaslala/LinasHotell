using LinasHotell.Controllers;
using Spectre.Console;

namespace LinasHotell.UIMenus
{

    public class RoomMenu
    {
        private readonly RoomController _roomController;

        public RoomMenu(RoomController roomController)
        {
            _roomController = roomController;
        }


        public async Task ShowRoomMenuAsync()
        {
            while (true)
            {
                var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Välj:")
                    .AddChoices(
                        "Visa alla rum",
                        "Skapa nytt rum",
                        "Uppdatera befintligt rum",
                        "Inaktivera rum",
                        "Tillbaka till lobbyn"));

                switch (choice)
                {
                    case "Visa alla rum":
                        await _roomController.ShowAllRoomsAsync();
                        break;

                    case "Skapa nytt rum":
                        await _roomController.AddRoomAsync();
                        break;

                    case "Uppdatera befintligt rum":
                        await _roomController.UpdateRoomAsync();
                        break;

                    case "Inaktivera rum":
                        await _roomController.SetBookableRoomStatusAsync();
                        break;

                    case "Tillbaka till lobbyn":
                        return;
                }
            }
            
        }
    }

    public enum RoomMenuResult
    {
        Stay,
        BackToLobby
    }
}
