namespace LinasHotell.Utilities
{
    using Spectre.Console;

    public static class CalendarRenderer
    {
        public static void Render(DateTime selectedDate)
        {
            var writer = new StringWriter();

            writer.WriteLine($"[yellow]{selectedDate:MMMM}[/]".ToUpper());
            writer.WriteLine("Mån  Tis  Ons  Tor  Fre  Lör  Sön");
            writer.WriteLine("────────────────────────────────");

            DateTime firstDay = new(selectedDate.Year, selectedDate.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(selectedDate.Year, selectedDate.Month);

            int startDay = ((int)firstDay.DayOfWeek + 6) % 7;

            for (int i = 0; i < startDay; i++)
                writer.Write("     ");

            for (int day = 1; day <= daysInMonth; day++)
            {
                if (day == selectedDate.Day)
                    writer.Write($"[green]{day,3}[/]  ");
                else
                    writer.Write($"{day,3}  ");

                if ((startDay + day) % 7 == 0)
                    writer.WriteLine();
            }

            var panel = new Panel(writer.ToString())
            {
                Border = BoxBorder.Rounded,
                Header = new PanelHeader($"{selectedDate:yyyy}", Justify.Center)
            };

            AnsiConsole.Write(Align.Center(panel));

            AnsiConsole.WriteLine();
            AnsiConsole.Write(Align.Center(
                new Markup("Använd piltangenter [blue]◄ ▲ ► ▼[/] för att navigera.\nTryck Enter för att välja datum.")
            ));
        }
    }
}
