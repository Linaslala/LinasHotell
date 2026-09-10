using Spectre.Console;

public enum MainMenuChoice
{
    Rum,
    Gäst,
    Bokning,
    Avsluta
}

public class MainMenu
{
    public Task<MainMenuChoice> ShowAsync()
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<MainMenuChoice>()
                .Title("Välj:")
                .AddChoices(
                    MainMenuChoice.Rum,
                    MainMenuChoice.Gäst,
                    MainMenuChoice.Bokning,
                    MainMenuChoice.Avsluta)
        );

        return Task.FromResult(choice);
    }
}
