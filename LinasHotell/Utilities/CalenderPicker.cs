namespace LinasHotell.Utilities
{
    using Spectre.Console;

    public static class CalendarPicker
    {
        public static DateTime PickDate(DateTime startDate, string title)
        {
            var minDate = DateTime.Today;
            var selectedDate = startDate < minDate ? minDate : startDate;

            while (true)
            {
                AnsiConsole.Clear();

                AnsiConsole.Write(
                    new Rule($"[yellow]{title}[/]")
                        .RuleStyle("grey")
                        .Centered()
                );

                CalendarRenderer.Render(selectedDate);

                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.Enter)
                    return selectedDate;

                if (key == ConsoleKey.Escape)
                    throw new OperationCanceledException();

                var next = CalendarNavigator.Navigate(selectedDate, key).Date;

                selectedDate = next < minDate ? minDate : next;
            }
        }
    }
}
