using Spectre.Console;

public enum MainMenuChoice
{
    Rum,
    Gäst,
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
                    MainMenuChoice.Avsluta)
        );

        return Task.FromResult(choice);
    }
}
